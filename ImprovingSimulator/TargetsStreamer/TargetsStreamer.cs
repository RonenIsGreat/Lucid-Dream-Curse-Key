using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client;
using System.Linq;
using System.Net;
using TargetsStreamerMain.Models;
using Newtonsoft.Json;

namespace TargetsStreamerMain
{
    public class TargetsStreamer
    {
        private readonly string _targetsRecordingPath;
        private static TargetsStreamer _instance;
        public static int MessagesSent { get; private set; }
        private readonly Stopwatch stopwatch;

        private static string RabbitMqHost;
        private static int RabbitMqPort;
        public static TargetsStreamer Instance
        {
            get
            {
                var targetsDataFilePath = ConfigurationManager.AppSettings["TargetsDataPath"];

                if (_instance == null)
                    _instance = new TargetsStreamer(targetsDataFilePath);
                return _instance;
            }
        }

        private TargetsStreamer(string targetsRecordingPath)
        {
            //Validity check
            IPAddress ip = IPAddress.Parse(ConfigurationManager.AppSettings["RabbitMqHost"]);
            int port = int.Parse(ConfigurationManager.AppSettings["RabbitMqPort"]);

            RabbitMqHost = ip.ToString();
            RabbitMqPort = port;

            MessagesSent = 0;
            stopwatch = new Stopwatch();
            _targetsRecordingPath = targetsRecordingPath;
        }

        private bool CheckRequirements()
        {
            if (!File.Exists(_targetsRecordingPath))
            {
                MessageBox.Show("Wrong path entered");
                return false;
            }

            return true;
        }

        public Task StartSending(CancellationToken ct)
        {
            stopwatch.Start();

            if (!CheckRequirements()) return Task.CompletedTask;
            using (var f = new StreamReader(File.OpenRead(_targetsRecordingPath), Encoding.UTF8))
            {
                short index = 0;
                var systemTracks = new SystemTarget
                {
                    sentTimeStamp = TimeType.ParseFromDateTime(DateTime.UtcNow)
                };
                //Take the array created in system tracks constructor
                var targetsDataList = systemTracks.systemTargets;

                while (!f.EndOfStream)
                {
                    //Check cancellation
                    if (ct.IsCancellationRequested)
                    {
                        return Task.CompletedTask;
                    }

                    var line = f.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line) && !string.IsNullOrEmpty(line))
                    {
                        var trackData = ParseTrackDataWithDelimiter(line);

                        // If array has 3 elemts, send the messages
                        if (index + 1 == 3)
                        {
                            targetsDataList.Add(trackData);

                            // Send each message after 1.3 secs
                            SpinWait.SpinUntil(() => stopwatch.ElapsedMilliseconds >= 1300 * MessagesSent);

                            SendMessage(systemTracks.ToByteArray());
                            SendMessageJSON(systemTracks);

                            MessagesSent++;

                            systemTracks = new SystemTarget
                            {
                                sentTimeStamp = TimeType.ParseFromDateTime(DateTime.UtcNow)
                            };
                            targetsDataList = systemTracks.systemTargets;

                            index = 0;
                        }
                        else
                        {
                            targetsDataList.Add(trackData);
                            index++;
                        }

                        // Send forever until canceled
                        if (f.EndOfStream && !ct.IsCancellationRequested)
                        {
                            f.BaseStream.Seek(0, SeekOrigin.Begin);
                        }

                    }

                }
            }
            return Task.CompletedTask;
        }

        private static TargetData ParseTrackDataWithDelimiter(string line)
        {
            var param = line.Split(',');
            var trackIdString = param[0];
            var bearingString = param[1];

            var trackId = long.Parse(trackIdString.Split(':')[1]);
            var relativeBearing = float.Parse(bearingString.Split(':')[1]);

            var trackData = new TargetData
            {
                relativeBearing = relativeBearing,
                trackID = trackId
            };
            return trackData;
        }


        private void SendMessage(byte[] message)
        {
            var factory = new ConnectionFactory { HostName = RabbitMqHost, RequestedHeartbeat = TimeSpan.FromSeconds(60), Port = RabbitMqPort };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("SystemTracks",
                    "direct", true);
                channel.BasicPublish("SystemTracks",
                    "SystemTracks",
                    null,
                    message);
            }
        }


        private void SendMessageJSON(SystemTarget targets)
        {
            var factory = new ConnectionFactory { HostName = RabbitMqHost, RequestedHeartbeat = TimeSpan.FromSeconds(60), Port = RabbitMqPort };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("SystemTracksJSON",
                    "direct", true);
                channel.BasicPublish("SystemTracksJSON",
                    "SystemTracks",
                    null,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(targets)));
            }
        }
    }
}
