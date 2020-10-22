using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client;
using System.Linq;
using TargetsStreamerMain.Models;

namespace TargetsStreamerMain
{
    public class TargetsStreamer
    {
        private readonly string _targetsRecordingPath;
        private static TargetsStreamer _instance;
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
            if (!CheckRequirements()) return Task.CompletedTask;
            using (var f = new StreamReader(File.OpenRead(_targetsRecordingPath), Encoding.UTF8))
            {
                short index = 0;
                var systemTracks = new SystemTracks
                {
                    sentTimeStamp = TimeType.ParseFromDateTime(DateTime.UtcNow)
                };
                //Take the array created in system tracks constructor
                var targetsDataList = systemTracks.systemTracks;

                while (!f.EndOfStream)
                {
                    //Check cancellation
                    if(ct.IsCancellationRequested)
                    {
                        return Task.CompletedTask;
                    }

                    var line = f.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line) && !string.IsNullOrEmpty(line))
                    {
                        var trackData = ParseTrackDataWithDelimiter(line);

                        // If array is full, send the messages
                        if (index + 1 == SystemTracks.ARRAY_SIZE)
                        {
                            targetsDataList[index] = trackData;

                            SendMessage(systemTracks.ToByteArray(), "SystemTracks");

                            index = 0;
                        }
                        else
                        {
                            targetsDataList[index] = trackData;
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

        private static TrackData ParseTrackDataWithDelimiter(string line)
        {
            var param = line.Split(',');
            var trackIdString = param[0];
            var bearingString = param[1];

            var trackId = long.Parse(trackIdString.Split(':')[1]);
            var relativeBearing = float.Parse(bearingString.Split(':')[1]);

            var trackData = new TrackData
            {
                relativeBearing = relativeBearing,
                trackID = trackId
            };
            return trackData;
        }


        private void SendMessage(byte[] message, string rKey)
        {
            var factory = new ConnectionFactory { HostName = "localhost", RequestedHeartbeat = TimeSpan.FromSeconds(60) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("SystemTracks",
                    "direct", true);
                channel.BasicPublish("channelControl",
                    rKey,
                    null,
                    message);
            }
        }


    }
}
