using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using NAudio.Wave;
using NAudio;
using NAudio.FileFormats.Wav;
using System.Threading.Tasks;
using AudioCreate.RestAPIController;

namespace AudioCreate
{
    struct subSegment
    {
        public byte[] message;
    }
    struct Segment
    {
        public subSegment[] subsegments;
    }
    public class AudioCreation
    {
        public static List<string> fileNames = new List<string>();

        private const int listenPort = 25104;
        private static SystemTarget Targets = new SystemTarget();
        private static string RabbitMqHost;
        private static int RabbitMqPort;
        private static Dictionary<long, WaveFileWriter> TargetWriters = new Dictionary<long, WaveFileWriter>();
        private static WaveFormat waveFormat = new WaveFormat(31250, 16, 1);
        private static string pathToAudioFile;

        private static void receiveTargets()
        {
            IPAddress ip = IPAddress.Parse(ConfigurationManager.AppSettings["RabbitMqHost"]);
            int port = int.Parse(ConfigurationManager.AppSettings["RabbitMqPort"]);

            RabbitMqHost = ip.ToString();
            RabbitMqPort = port;
            var factory = new ConnectionFactory() { HostName = RabbitMqHost, Port = RabbitMqPort };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "SystemTracks", "direct", true);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: "SystemTracks",
                                  routingKey: "SystemTracks");

                Console.WriteLine(" [*] Waiting for logs.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    SystemTarget message = SystemTarget.FromByteArray(body);
                    Targets = message;
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static void StartListener()
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            subSegment tempSub = new subSegment();
            Segment tempSegment = new Segment();
            tempSegment.subsegments = new subSegment[10];
            pathToAudioFile = GetAudioFolderPath();
            double heading;
            int idNumber;
            int counter = 1;
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);
                    idNumber = Convert.ToInt32(bytes[4]);
                    if (idNumber == counter)
                    {
                        tempSub.message = bytes;
                        tempSegment.subsegments[counter - 1] = tempSub;
                        if (counter == 10)
                        {
                            byte[] segBytes = SegmentToBytes(tempSegment);
                            heading = getHeading(segBytes);
                            foreach (TargetData targetData in Targets.systemTargets)
                            {
                                byte[] tAudio = getTargetAudio(segBytes, targetData.relativeBearing, heading);
                                WaveFileWriter writer = getWriter(targetData.trackID);
                                writer.Write(tAudio, 0, tAudio.Length);
                                writer.Flush();
                            }
                            counter = 1;
                        }
                        else
                            counter++;
                    }
                    else
                    {
                        counter = 1;
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }

        private static WaveFileWriter getWriter(long tID)
        {

            if (!(TargetWriters.ContainsKey(tID)))
            {
                //WaveFileWriter newWriter = new WaveFileWriter(pathToAudioFile, waveFormat);
                string filename = pathToAudioFile+"/" + tID + ".wav";
                fileNames.Add(tID + ".wav");
                    WaveFileWriter newWriter = new WaveFileWriter(filename, waveFormat);
                    TargetWriters.Add(tID, newWriter);

            }
            return TargetWriters[tID];
        }

        private static double getHeading(byte[] casBeambusSegment)
        {
            byte[] heading = new byte[64];
            Array.Copy(casBeambusSegment, 13856, heading, 0, 64);
            return BitConverter.ToDouble(heading, 0);
        }

        private static byte[] SegmentToBytes(Segment segment)
        {
            byte[] ByteArray = new byte[14000];
            for (int i = 0; i < 10; i++)
            {
                Array.Copy(segment.subsegments[i].message, 0, ByteArray, i * 1400, 1400);
            }
            return ByteArray;
        }

        private static byte[] getTargetAudio(byte[] casBeambusSegment, double targetRelativeBearing, double ownHeading)
        {
            double trackDegree = (ownHeading + targetRelativeBearing) % 360;
            const double factor = 192.0 / 360.0;
            double beamNumber = trackDegree * factor;

            // Simplified version, select nearest CAS beam to the target and return it
            int selectedBeamNumber = (int)Math.Round(beamNumber);

            byte[] targetAudio = GetBeam(casBeambusSegment, selectedBeamNumber);
            return targetAudio;
        }


        static Dictionary<int, BeamsInSubSegmentENUM> m_beamsAndSubSegmentsDictionary;
        enum BeamsInSubSegmentENUM
        {
            SubSegment1 = 0,
            SubSegment1_2 = 1,
            SubSegment2 = 2,
            SubSegment2_3 = 3,
            SubSegment3 = 4,
            SubSegment3_4 = 5,
            SubSegment4 = 6,
            SubSegment4_5 = 7,
            SubSegment5 = 8,
            SubSegment5_6 = 9,
            SubSegment6 = 10,
            SubSegment6_7 = 11,
            SubSegment7 = 12,
            SubSegment7_8 = 13,
            SubSegment8 = 14,
            SubSegment8_9 = 15,
            SubSegment9 = 16,
            SubSegment9_10 = 17,
            SubSegment10 = 18,
        }

        // Init the dictionary from beam number to the segments that contains it
        private static void dictionaryInit()
        {
            m_beamsAndSubSegmentsDictionary = new Dictionary<int, BeamsInSubSegmentENUM>();
            for (int i = 1; i <= 17; i++)
            {
                m_beamsAndSubSegmentsDictionary.Add(i, BeamsInSubSegmentENUM.SubSegment1);
            }
            m_beamsAndSubSegmentsDictionary.Add(18, BeamsInSubSegmentENUM.SubSegment1_2);

            for (int i = 19; i <= 39; i++)
            {
                m_beamsAndSubSegmentsDictionary.Add(i, BeamsInSubSegmentENUM.SubSegment2);
            }
            m_beamsAndSubSegmentsDictionary.Add(40, BeamsInSubSegmentENUM.SubSegment2_3);

            for (int i = 41; i <= 60; i++)
            {
                m_beamsAndSubSegmentsDictionary.Add(i, BeamsInSubSegmentENUM.SubSegment3);
            }
            m_beamsAndSubSegmentsDictionary.Add(61, BeamsInSubSegmentENUM.SubSegment3_4);

            for (int i = 62; i <= 82; i++)
            {
                m_beamsAndSubSegmentsDictionary.Add(i, BeamsInSubSegmentENUM.SubSegment4);
            }

            for (int i = 83; i <= 103; i++)
            {
                m_beamsAndSubSegmentsDictionary.Add(i, BeamsInSubSegmentENUM.SubSegment5);
            }
            m_beamsAndSubSegmentsDictionary.Add(104, BeamsInSubSegmentENUM.SubSegment5_6);

            for (int i = 105; i <= 124; i++)
            {
                m_beamsAndSubSegmentsDictionary.Add(i, BeamsInSubSegmentENUM.SubSegment6);
            }
            m_beamsAndSubSegmentsDictionary.Add(125, BeamsInSubSegmentENUM.SubSegment6_7);

            for (int i = 126; i <= 146; i++)
            {
                m_beamsAndSubSegmentsDictionary.Add(i, BeamsInSubSegmentENUM.SubSegment7);
            }
            m_beamsAndSubSegmentsDictionary.Add(147, BeamsInSubSegmentENUM.SubSegment7_8);

            for (int i = 148; i <= 167; i++)
            {
                m_beamsAndSubSegmentsDictionary.Add(i, BeamsInSubSegmentENUM.SubSegment8);
            }
            m_beamsAndSubSegmentsDictionary.Add(168, BeamsInSubSegmentENUM.SubSegment8_9);

            for (int i = 169; i <= 188; i++)
            {
                m_beamsAndSubSegmentsDictionary.Add(i, BeamsInSubSegmentENUM.SubSegment9);
            }
            m_beamsAndSubSegmentsDictionary.Add(189, BeamsInSubSegmentENUM.SubSegment9_10);

            for (int i = 190; i < 193; i++)
            {
                m_beamsAndSubSegmentsDictionary.Add(i, BeamsInSubSegmentENUM.SubSegment10);
            }
        }

        // Extract beam byte array from the CAS beambus array
        private static byte[] GetBeam(byte[] beamBusCasData, int beamNumber)
        {
            byte[] beamToReturn = new byte[64];

            switch (m_beamsAndSubSegmentsDictionary[beamNumber])
            {
                case BeamsInSubSegmentENUM.SubSegment1:
                    Array.Copy(beamBusCasData, 256 + (64 * (beamNumber - 1)), beamToReturn, 0, 64);
                    break;

                case BeamsInSubSegmentENUM.SubSegment1_2:
                    Array.Copy(beamBusCasData, 1344, beamToReturn, 0, 56);
                    Array.Copy(beamBusCasData, 1432, beamToReturn, 56, 8);

                    break;

                case BeamsInSubSegmentENUM.SubSegment2:
                    Array.Copy(beamBusCasData, 1440 + (64 * (beamNumber - 19)), beamToReturn, 0, 64);
                    break;

                case BeamsInSubSegmentENUM.SubSegment2_3:
                    Array.Copy(beamBusCasData, 2784, beamToReturn, 0, 16);
                    Array.Copy(beamBusCasData, 2832, beamToReturn, 16, 48);
                    break;

                case BeamsInSubSegmentENUM.SubSegment3:
                    Array.Copy(beamBusCasData, 2880 + (64 * (beamNumber - 41)), beamToReturn, 0, 64);
                    break;

                case BeamsInSubSegmentENUM.SubSegment3_4:
                    Array.Copy(beamBusCasData, 4160, beamToReturn, 0, 40);
                    Array.Copy(beamBusCasData, 4232, beamToReturn, 40, 24);

                    break;

                case BeamsInSubSegmentENUM.SubSegment4:
                    Array.Copy(beamBusCasData, 4257 + (64 * (beamNumber - 62)), beamToReturn, 0, 64);
                    break;

                case BeamsInSubSegmentENUM.SubSegment4_5:
                    break;

                case BeamsInSubSegmentENUM.SubSegment5:
                    Array.Copy(beamBusCasData, 5632 + (64 * (beamNumber - 83)), beamToReturn, 0, 64);
                    break;

                case BeamsInSubSegmentENUM.SubSegment5_6:
                    Array.Copy(beamBusCasData, 6976, beamToReturn, 0, 24);
                    Array.Copy(beamBusCasData, 7032, beamToReturn, 24, 40);
                    break;

                case BeamsInSubSegmentENUM.SubSegment6:
                    Array.Copy(beamBusCasData, 7072 + (64 * (beamNumber - 105)), beamToReturn, 0, 64);
                    break;

                case BeamsInSubSegmentENUM.SubSegment6_7:
                    Array.Copy(beamBusCasData, 8352, beamToReturn, 0, 48);
                    Array.Copy(beamBusCasData, 8432, beamToReturn, 48, 16);
                    break;

                case BeamsInSubSegmentENUM.SubSegment7:
                    Array.Copy(beamBusCasData, 8448 + (64 * (beamNumber - 126)), beamToReturn, 0, 64);
                    break;

                case BeamsInSubSegmentENUM.SubSegment7_8:
                    Array.Copy(beamBusCasData, 9792, beamToReturn, 0, 8);
                    Array.Copy(beamBusCasData, 9832, beamToReturn, 8, 56);
                    break;

                case BeamsInSubSegmentENUM.SubSegment8:
                    Array.Copy(beamBusCasData, 9888 + (64 * (beamNumber - 148)), beamToReturn, 0, 64);
                    break;

                case BeamsInSubSegmentENUM.SubSegment8_9:
                    Array.Copy(beamBusCasData, 11168, beamToReturn, 0, 32);
                    Array.Copy(beamBusCasData, 11232, beamToReturn, 32, 32);
                    break;

                case BeamsInSubSegmentENUM.SubSegment9:
                    Array.Copy(beamBusCasData, 11264 + (64 * (beamNumber - 169)), beamToReturn, 0, 64);
                    break;

                case BeamsInSubSegmentENUM.SubSegment9_10:
                    Array.Copy(beamBusCasData, 12544, beamToReturn, 0, 56);
                    Array.Copy(beamBusCasData, 12632, beamToReturn, 56, 8);
                    break;

                case BeamsInSubSegmentENUM.SubSegment10:
                    Array.Copy(beamBusCasData, 12640 + (64 * (beamNumber - 190)), beamToReturn, 0, 64);
                    break;
            }

            return beamToReturn;
        }
        static string GetProjectPath()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            for (int i = 0; i < 3; i++)
            {
                System.IO.DirectoryInfo directoryInfo = System.IO.Directory.GetParent(path);
                path = directoryInfo.FullName;
            }
            return path; 
        }
        static string GetAudioFolderPath()
        {
            string path = GetProjectPath();
            string newStr = path.Replace('\\','/');
            newStr = newStr + "/channels-control-panel/public";
            return newStr;
        }

        public static async Task Main()
        {
            //string path = System.IO.Directory.GetCurrentDirectory();
            //path = GetProjectPath(path);
            //Console.WriteLine(path);
            //Console.ReadLine();

            dictionaryInit();
            Thread A = new Thread(StartListener);
            Thread B = new Thread(receiveTargets);
            A.Start();
            B.Start();
            //RestApiServer restApiServer = new RestApiServer(5555);
            //await restApiServer.StartAsync();
        }
    }
}
