using System;
using GlobalResourses;
using System.Threading;

namespace DisplayingLiveStreams
{
    class DisplayApp
    {
        public static void Run()
        {
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
                _UdpListeners[i].OpenQChannel();

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

        }//End Main

    }//End Program
}
