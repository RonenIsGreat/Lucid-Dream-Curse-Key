using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GlobalResourses;
using LiveStreamsDisplay;
using SaveStream;
using System.Configuration;

namespace Lucid_Dream_Backend
{
    class Program
    {
        static SaveStreamHelper saveStreamHelper;
        static void Main(string[] args)
        {
            //Initlize and get the save stream helper instance
            var savePath = ConfigurationManager.AppSettings["save-path"];
            saveStreamHelper = SaveStreamHelper.Instance;
            saveStreamHelper.InitSaveStreamHelper(savePath);

            //Initializing The Ports
            Port[] _Ports = new Port[6];

            _Ports[0] = new Port(ChannelNames.CasStave, 25101);
            _Ports[1] = new Port(ChannelNames.FasTasStave, 25102);
            _Ports[2] = new Port(ChannelNames.PRSStave, 25103);
            _Ports[3] = new Port(ChannelNames.CasBeam, 25104);
            _Ports[4] = new Port(ChannelNames.FasTasBeam, 25105);
            _Ports[5] = new Port(ChannelNames.IDRSBus, 25106);

            UDPListener[] _UdpListeners = new UDPListener[6];

            for (int i = 0; i < _UdpListeners.Length; i++)
            {
                _UdpListeners[i] = new UDPListener(_Ports[i]);
                _UdpListeners[i].OnDataReceived += onDataReceived;

            }//End For

            Thread _CasStaveThread = new Thread(_UdpListeners[0].StartListener);
            Thread _FasTasStaveThread = new Thread(_UdpListeners[1].StartListener);
            Thread _PrsStaveThread = new Thread(_UdpListeners[2].StartListener);
            Thread _CasBeamThread = new Thread(_UdpListeners[3].StartListener);
            Thread _FasTasBeamThread = new Thread(_UdpListeners[4].StartListener);
            Thread _AtmIdrsThread = new Thread(_UdpListeners[5].StartListener);

            Console.WriteLine("Live Streams :");

            //Start Listening To The Ports
            //  _CasStaveThread.Start();
            // _FasTasStaveThread.Start();
            // _PrsStaveThread.Start();
            _CasBeamThread.Start();
            _FasTasBeamThread.Start();
            _AtmIdrsThread.Start();
            while (true)
            {
                Thread.Sleep(100);
            }

        }//End Main

        private static void onDataReceived(object sender, byte[] data)
        {
            bool succeeded = saveStreamHelper.SaveData(data, DateTime.Now.ToShortDateString());
            if (!succeeded)
            {
                Console.WriteLine("Failed to save message");
            }
            //Data goes in here
        }
    }//End Program

}//End Lucid_Dream_Backend
