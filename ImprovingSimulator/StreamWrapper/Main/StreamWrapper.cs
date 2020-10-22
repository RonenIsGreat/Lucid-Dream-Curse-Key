using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UdpSocket.Main;

namespace StreamWrapper.Main
{
    public sealed class StreamWrapper
    {
        private readonly UDPSocket client;
        private readonly double delimiter;
        private readonly Stopwatch stopwatch;
        private long subSegmentLength;
        private readonly string subSementPath;

        public StreamWrapper(string path, string ipAddress, int port, double delimiter)
        {
            this.delimiter = delimiter;
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
        ///     Send Messages with given reregistration token and number of messages to send. <br></br>
        ///     If Number of messages is -1 then it will send forever until canceled with <see cref="CancellationToken" />
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="delimiter"></param>
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

                var numberOfMessagesInRecording = (limitInBytes - limitInBytes % 1400) / 1400;

                for (var messageIndex = 0; messageIndex < numberOfMessagesInRecording; messageIndex++)
                {
                    var index = messageIndex;
                    // Waits for elapsed milliseconds condition safely
                    SpinWait.SpinUntil(() => stopwatch.ElapsedMilliseconds >= 1.024 / delimiter * index);

                    if (ct.IsCancellationRequested)
                    {
                        return Task.CompletedTask;
                    }

                    var buffer = new byte[1400];
                    f.Read(buffer, 0, 1400);
                    client.Send(buffer);

                    //Sets the index according to numberOfMessagesToSend (forever or limited)
                    messageIndex = CheckSendLimit(messageIndex++, numberOfMessagesInRecording, f,
                        numberOfMessagesToSend);
                }
            }

            return Task.CompletedTask;
        }


        private int CheckSendLimit(int messageIndex, long limit, Stream f, long numberOfMessagesToSend)
        {
            if (messageIndex == limit && numberOfMessagesToSend == -1)
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
            else if (messageIndex == limit && numberOfMessagesToSend != -1) f.Flush();

            return messageIndex;
        }
    } //End BeamBusCasSender
}