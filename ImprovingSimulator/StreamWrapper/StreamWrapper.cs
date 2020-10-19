using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using UdpSocket;
using Task = System.Threading.Tasks.Task;
using Timer = System.Timers.Timer;

namespace StreamWrapper
{
    public sealed class StreamWrapper
    {
        private long subSegmentLength;
        private string subSementPath;
        private UDPSocket client;
        private Stopwatch stopwatch;
        private Timer testTimer;
        private int count;

         public StreamWrapper(string path, string ipAddress, int port)
         {
             count = 0;
             subSementPath = path;
            stopwatch = new Stopwatch();
            client = new UDPSocket(ipAddress, port);
            testTimer = new Timer(1000);
            testTimer.Elapsed += TestTimerOnElapsed;
         }

         private void TestTimerOnElapsed(object sender, ElapsedEventArgs e)
         {
             Console.WriteLine("Times: " + count);
         }

        private bool CheckRequirements(long numberOfMessagesToSend)
         {
             if (!File.Exists(subSementPath))
             {
                 MessageBox.Show("Wrong path entered");
                 return false;
             }

             if (numberOfMessagesToSend > subSegmentLength)
             {
                 MessageBox.Show("Number of messages to send is greater than total messages in recording file",
                     "Exceeds total count");
                 return false;
             }

             return true;
         }

         /// <summary>
         /// Send Messages with given reregistration token and number of messages to send. <br></br>
         /// If Number of messages is -1 then it will send forever until canceled with <see cref="CancellationToken"/>
         /// </summary>
         /// <param name="ct"></param>
         /// <param name="delimiter"></param>
         /// <param name="numberOfMessagesToSend"></param>
         /// <returns></returns>
         public Task SendMessages(CancellationToken ct, double delimiter = 1, long numberOfMessagesToSend = -1)
         {
             testTimer.Start();

             // Check requirements before sending messages
             if (!CheckRequirements(numberOfMessagesToSend)) return Task.CompletedTask;
             // Encapsulate with using so resources will be flushed after execution
             using (var f = File.OpenRead(subSementPath))
            {
                subSegmentLength = f.Length;

                // Sending limit, can be either numberOfMessagesToSend or recording file size ( will be looped around forever if so)
                var limitInBytes = numberOfMessagesToSend > 0 ? numberOfMessagesToSend * 1400 : subSegmentLength;

                stopwatch.Start();

                var numberOfMessagesInRecording = (limitInBytes  - limitInBytes % 1400) / 1400;

                for (var messageIndex = 0; messageIndex < numberOfMessagesInRecording; messageIndex++)
                {
                    var index = messageIndex;
                    // Waits for elapsed milliseconds condition safely
                    SpinWait.SpinUntil(() => stopwatch.ElapsedMilliseconds >= (1.024 / delimiter) * index);

                    if (ct.IsCancellationRequested)
                    {
                        count = 0;
                        return Task.CompletedTask;
                    }

                    var buffer = new byte[1400];
                    f.Read(buffer, 0, 1400);
                    client.Send(buffer);
                    count++;

                    //Sets the index according to numberOfMessagesToSend (forever or limited)
                    messageIndex = CheckSendLimit(messageIndex + 1, numberOfMessagesInRecording, f, numberOfMessagesToSend);
                }
            }
            return Task.CompletedTask;

         }


         private int CheckSendLimit(int messageIndex, long limit, Stream f, long numberOfMessagesToSend)
         {
             if (messageIndex  == limit && numberOfMessagesToSend == -1)
             {
                 try
                 {
                     f.Flush();
                     // Will be equal to 0 once incremented in the for loop
                     messageIndex = -1;
                     f.Seek(0, SeekOrigin.Begin);
                 }
                catch (Exception)
                {
                     // ignored
                }

             }
             else if (messageIndex == limit && numberOfMessagesToSend != -1)
             {
                f.Flush();
             }

             return messageIndex;
         }
    }//End BeamBusCasSender
}
