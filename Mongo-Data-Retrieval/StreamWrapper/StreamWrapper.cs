using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UdpSocket;

namespace StreamWrapper
{
    public sealed class StreamWrapper
    {
        private readonly UDPSocket client;
        private readonly double delimiter;
        private readonly Stopwatch stopwatch;
        private long subSegmentLength;
        private readonly string subSementPath;
        private readonly JArray casResults;

        public StreamWrapper(string ipAddress, int port, double delimiter, JArray casResults)
        {
            this.delimiter = delimiter;
            stopwatch = new Stopwatch();
            client = new UDPSocket(ipAddress, port);
            this.casResults = casResults;
        }

        /// <summary>
        ///     Send Messages with given reregistration token and number of messages to send. <br></br>
        ///     If Number of messages is -1 then it will send forever until canceled with <see cref="CancellationToken" />
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public Task SendMessages(CancellationToken ct)
        {

            // Sending limit, can be either numberOfMessagesToSend or recording file size ( will be looped around forever if so)
            //var limitInBytes = numberOfMessagesToSend * 1400;

            stopwatch.Start();

            //var numberOfMessagesInRecording = (limitInBytes - limitInBytes % 1400) / 1400;
            var index = 0;

            foreach (var res in casResults)
            {
                var numOfMessagesInRes = res["NumOfMessages"];

                foreach (var message in res["Messages"])
                {
                    //var index = messageIndex;
                    // Waits for elapsed milliseconds condition safely
                    SpinWait.SpinUntil(() => stopwatch.ElapsedMilliseconds >= 1.024 / delimiter * index);

                    if (ct.IsCancellationRequested)
                    {
                        return Task.CompletedTask;
                    }

                    //var buffer = new byte[1400];
                    //client.Send(message);
                    Console.WriteLine(message["TimeStamp"]);
                    client.Send(message["Data"]["$binary"]["base64"].ToObject<byte[]>());

                    index++;
                }
            }

            return Task.CompletedTask;
        }
    } //End BeamBusCasSender
}
