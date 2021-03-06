﻿using System;
using System.Configuration;
using System.Threading;
using DBManager;
using GlobalResourses;
using UDPListener;

namespace Lucid_Dream_Backend
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Initializing The Ports
            var _Ports = new ChannelDetails[6];

            _Ports[0] = new ChannelDetails(ChannelNames.CasStave, 25101);
            _Ports[1] = new ChannelDetails(ChannelNames.FasTasStave, 25102);
            _Ports[2] = new ChannelDetails(ChannelNames.PRSStave, 25103);
            _Ports[3] = new ChannelDetails(ChannelNames.CasBeam, 25104);
            _Ports[4] = new ChannelDetails(ChannelNames.FasTasBeam, 25105);
            _Ports[5] = new ChannelDetails(ChannelNames.IDRSBus, 25106);

            var udpListeners = new UdpListener[6];
            var consumers = new Consumer.Consumer[6];

            for (var i = 0; i < udpListeners.Length; i++)
            {
                udpListeners[i] = new UdpListener(_Ports[i]);
                consumers[i] = new Consumer.Consumer(udpListeners[i]);
                consumers[i].ListenToQueue();
            } //End For


            Console.WriteLine("Live Streams :");
            Controller.Controller.RunAsync();

            while (true)
            {
                Thread.Sleep(100);
            }
        } //End Main
    } //End Program
} //End Lucid_Dream_Backend