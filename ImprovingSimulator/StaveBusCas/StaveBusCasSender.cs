using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Timers;

namespace StaveBusCas
{
    public static class StaveBusCasSender
    {
        static int subSegmentNum;
        static byte[][] subSements;
        static UDPSocket client;

        public static void SendMessage()
        {
            int messageCount = 0;
            System.Timers.Timer aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += (sender, e) => MyElapsedMethod(sender, e, messageCount);
            int count = 0;

            Stopwatch stopwatch = new Stopwatch();
            subSements = FileEdit.GetRecording(Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, Properties.Settings.Default.Recording_path));
            client = new UDPSocket();
            client.Client(Properties.Settings.Default.IP,
                Properties.Settings.Default.Port);
            subSegmentNum = subSements.Length;
            stopwatch.Start();
            aTimer.Start();
            while (count * 6 < subSegmentNum - 6)
            {
                if (stopwatch.ElapsedMilliseconds >= 1.024 * count)
                {

                    for (int j = count * 6; j < count * 6 + 6; j++)
                    {
                        client.Send(subSements[j]);
                        messageCount++;
                    }
                    count++;

                }

            }
        }
        public static void SendNumberOfMessages(int number)
        {
            int messageCount = 0;
            //  System.Timers.Timer aTimer = new System.Timers.Timer(1000);
            //  aTimer.Elapsed += (sender, e) => MyElapsedMethod(sender, e, messageCount);
            int count = 0;

            Stopwatch stopwatch = new Stopwatch();
            subSements = FileEdit.GetRecording(Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, Properties.Settings.Default.Recording_path));
            client = new UDPSocket();
            client.Client(Properties.Settings.Default.IP,
                Properties.Settings.Default.Port);
            subSegmentNum = subSements.Length;
            stopwatch.Start();
            int limit;
            //  aTimer.Start();
            while (count * 6 < number - 6)
            {

                if (stopwatch.ElapsedMilliseconds >= 1.024 * count)
                {
                    if (count * 6 + 6 > number)
                        limit = number;
                    else
                        limit = count * 6 + 6;
                    for (int j = count * 6; j < limit; j++)
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

    }//End BeamBusCasSender

}//End BeamBusCas


