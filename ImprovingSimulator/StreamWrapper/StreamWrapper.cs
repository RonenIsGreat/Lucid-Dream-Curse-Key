using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UdpSocket;

namespace StreamWrapper
{
    public class StreamWrapper
    {
         private int subSegmentLength;
         private byte[][] subSements;
         private UDPSocket client;
         private Stopwatch stopwatch;
         private uint numOfMessages;

        public StreamWrapper(string path, string ipAddress, int port)
        {
            subSements = GetRecording(path);
            stopwatch = new Stopwatch();
            client = new UDPSocket(ipAddress, port);
        }

        public void SendMessages(ushort delimiter, int numberOfMessagesToSend = -1)
        {
            subSegmentLength = subSements.Length;
            stopwatch.Start();

            while ((numOfMessages * delimiter) < (numberOfMessagesToSend == -1 ? subSegmentLength - delimiter : numberOfMessagesToSend - delimiter))
            {
                if (stopwatch.ElapsedMilliseconds >= 1.024 * numOfMessages)
                {

                    for (var j = numOfMessages * delimiter; j < numOfMessages * delimiter + delimiter; j++)
                    {
                        client.Send(subSements[j]);
                        numOfMessages++;
                    }
                    numOfMessages++;

                }

            }
        }
        public void SendNumberOfMessages(int number)
        {
            int messageCount = 0;
            System.Timers.Timer aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += (sender, e) => MyElapsedMethod(sender, e, messageCount);
            int count = 0;

            Stopwatch stopwatch = new Stopwatch();
            subSements = GetRecording(Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, Properties.Settings.Default.Recording_path));
            client = new UDPSocket();
            client.Client(Properties.Settings.Default.IP,
                Properties.Settings.Default.Port);
            subSegmentLength = subSements.Length;
            stopwatch.Start();
            int limit;
            aTimer.Start();
            while (count * 7 < number - 7)
            {

                if (stopwatch.ElapsedMilliseconds >= 1.024 * count)
                {
                    if (count * 7 + 7 > number)
                        limit = number;
                    else
                        limit = count * 7 + 7;
                    for (int j = count * 7; j < limit; j++)
                    {
                        client.Send(subSements[j]);
                        messageCount++;
                    }
                    count++;

                }

            }
        }

        private static void MyElapsedMethod(Object source, ElapsedEventArgs e, int messageCount)
        {
            Console.WriteLine(messageCount);

        }

        private static void delayInMs(double ms)
        {
            for (int i = 0; i < ms * 480000; i++)
            {
                //Delay

            }//For

        }//End delayInMs

        public static byte[][] GetRecording(string path)
        {
            int subSegmentsNum;
            byte[] casBeamBusRecording;
            byte[][] SubSegments;

            casBeamBusRecording = File.ReadAllBytes(path);
            subSegmentsNum = casBeamBusRecording.Length / 1400;
            SubSegments = new byte[subSegmentsNum][];

            for (int i = 0; i < subSegmentsNum; i++)
            {
                SubSegments[i] = new byte[1400];
                Array.Copy(casBeamBusRecording, i * 1400, SubSegments[i], 0, 1400);

            }//End For

            return (SubSegments);

        }//End GetRecording


    }//End BeamBusCasSender
}
