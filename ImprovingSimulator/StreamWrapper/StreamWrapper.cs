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

namespace StreamWrapper
{
    public sealed class StreamWrapper
    {
        private long subSegmentLength;
        private string subSementPath;
        private UDPSocket client;
        private Stopwatch stopwatch;

         public StreamWrapper(string path, string ipAddress, int port)
         {
             subSementPath = path;
            stopwatch = new Stopwatch();
            client = new UDPSocket(ipAddress, port);
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
         /// <param name="numberOfMessagesToSend"></param>
         /// <returns></returns>
         public Task SendMessages(CancellationToken ct, long numberOfMessagesToSend = -1)
         {
             // Check requirements before sending messages
             if (!CheckRequirements(numberOfMessagesToSend)) return Task.CompletedTask;
             // Encapsulate with using so resources will be flushed after execution
             using (var f = File.OpenRead(subSementPath))
            {
                subSegmentLength = f.Length;

                // Sending limit, can be either numberOfMessagesToSend or recording file size ( will be looped around forever if so)
                var limitInBytes = numberOfMessagesToSend > 0 ? numberOfMessagesToSend * 1400 : subSegmentLength;

                stopwatch.Start();

                //NOTE: In original implantation, we send x amount of messages every 1.024 ms.
                // I changed it to send Each message after 1.024 ms. (until decided otherwise)
                // Because of that, the delimiter ( 6, 10, 7 etc) has been deleted.
                var numberOfMessagesInRecording = (limitInBytes  - limitInBytes % 1400) / 1400;

                for (var messageIndex = 0; messageIndex < numberOfMessagesInRecording; messageIndex++)
                {
                    var index = messageIndex;
                    // Waits for elapsed milliseconds condition safely
                    SpinWait.SpinUntil(() => stopwatch.ElapsedMilliseconds >= 1.024 * index);

                    if (ct.IsCancellationRequested)
                    {
                        return Task.CompletedTask;
                    }

                    var buffer = new byte[1400];
                    f.Read(buffer, 0, 1400);
                    client.Send(buffer);

                    //Sets the index according to numberOfMessagesToSend (forever or limited)
                    messageIndex = CheckSendLimit(messageIndex, limitInBytes, f);
                }
            }
            return Task.CompletedTask;

         }

         private int CheckSendLimit(int messageIndex, long limit, FileStream f)
         {
             if (messageIndex == limit)
             {
                 try
                 {
                     f.Flush();
                 }
                 catch (Exception)
                 {
                     // ignored
                 }

                 // If limit is equal to length of recording file (send forever) then reset messageIndex to continue sending.
                 // Else do nothing and exit the loop respectively 
                 messageIndex = limit == subSegmentLength ? 0 : messageIndex;
             }

             return messageIndex;
         }
    }//End BeamBusCasSender
}
