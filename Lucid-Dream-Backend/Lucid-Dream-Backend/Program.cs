﻿using System;
using System.Configuration;
using Controller;
using GlobalResourses;
using UDPListener;

namespace Lucid_Dream_Backend
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Initlize and get the save stream helper instance
            var savePath = ConfigurationManager.AppSettings["save-path"];

            //Initializing The Ports
            var _Ports = new ChannelDetails[6];

            _Ports[0] = new ChannelDetails(ChannelNames.CasStave, 25101);
            _Ports[1] = new ChannelDetails(ChannelNames.FasTasStave, 25102);
            _Ports[2] = new ChannelDetails(ChannelNames.PRSStave, 25103);
            _Ports[3] = new ChannelDetails(ChannelNames.CasBeam, 25104);
            _Ports[4] = new ChannelDetails(ChannelNames.FasTasBeam, 25105);
            _Ports[5] = new ChannelDetails(ChannelNames.IDRSBus, 25106);

            UdpListener[] udpListeners = new UdpListener[6];
            Consumer[] consumers = new Consumer[6];

            for (var i = 0; i < udpListeners.Length; i++)
            {
                udpListeners[i] = new UdpListener(_Ports[i]);
               consumers[i] = new Consumer(udpListeners[i], savePath);
                consumers[i].ListenToQueue();
            } //End For


            Console.WriteLine("Live Streams :");
            Controller.Controller.Run();
        } //End Main
    } //End Program
} //End Lucid_Dream_Backend