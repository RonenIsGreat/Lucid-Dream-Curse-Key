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

namespace StaveBusFasTas
{
    public static class StaveBusFasTasSender
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
            while (count * 7 < subSegmentNum)
            {
                if (stopwatch.ElapsedMilliseconds >= 1.024 * count)
                {

                    for (int j = count * 7; j < count * 7 + 7; j++)
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
            int limit;
            aTimer.Start();
            while (count * 7 < number)
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

    }//End BeamBusCasSender

}//End BeamBusCas


