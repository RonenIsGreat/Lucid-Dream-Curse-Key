using System;
using System.Configuration;
using System.Threading;
using GlobalResourses;
using UDPListener;

namespace Lucid_Dream_Backend
{
    internal class Program
    {
        private static void Main()
        {
            InitializeVariables(out string dbConnectionUrl,
                                out UdpListener[] udpListeners,
                                out Consumer.Consumer[] consumers);

            ChannelDetails[] _Ports = SetupChannels();

            SetupConsumers(dbConnectionUrl, udpListeners, consumers, _Ports);

            StartControllerAndWait();
        } //End Main

        private static void StartControllerAndWait()
        {
            Console.WriteLine("Live Streams :");
            Controller.Controller.RunAsync();

            while (true)
            {
                Thread.Sleep(100);
            }
        }

        private static void SetupConsumers(string dbConnectionUrl, UdpListener[] udpListeners, Consumer.Consumer[] consumers, ChannelDetails[] _Ports)
        {
            for (var i = 0; i < udpListeners.Length; i++)
            {
                udpListeners[i] = new UdpListener(_Ports[i]);
                consumers[i] = new Consumer.Consumer(udpListeners[i], dbConnectionUrl);
                consumers[i].ListenToQueue();
            } //End For
        }

        private static ChannelDetails[] SetupChannels()
        {
            //Initializing The Ports
            var _Ports = new ChannelDetails[6];

            _Ports[0] = new ChannelDetails(ChannelNames.CasStave, 25101);
            _Ports[1] = new ChannelDetails(ChannelNames.FasTasStave, 25102);
            _Ports[2] = new ChannelDetails(ChannelNames.PRSStave, 25103);
            _Ports[3] = new ChannelDetails(ChannelNames.CasBeam, 25104);
            _Ports[4] = new ChannelDetails(ChannelNames.FasTasBeam, 25105);
            _Ports[5] = new ChannelDetails(ChannelNames.IDRSBus, 25106);
            return _Ports;
        }

        private static void InitializeVariables(out string dbConnectionUrl, out UdpListener[] udpListeners, out Consumer.Consumer[] consumers)
        {
            //Initialize and get the save stream helper instance
            dbConnectionUrl = ConfigurationManager.AppSettings["db-url"];
            udpListeners = new UdpListener[6];
            consumers = new Consumer.Consumer[6];
        }
    } //End Program
} //End Lucid_Dream_Backend