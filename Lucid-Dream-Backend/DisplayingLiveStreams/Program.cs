using System;
using GlobalResourses;
using System.Threading;

namespace DisplayingLiveStreams
{
    class Program
    {
        static void Main(string[] args)
        {
            //Initializing The Ports
            Port[] _Ports = new Port[6];

            _Ports[0] = new Port("CAS Stave", 25101);
            _Ports[1] = new Port("FAS/TAS Stave", 25102);
            _Ports[2] = new Port("PRS Stave", 25103);
            _Ports[3] = new Port("CAS Beam", 25104);
            _Ports[4] = new Port("FAS/TAS Beam", 25105);
            _Ports[5] = new Port("ATM IDRS", 25106);

            UDPListener[] _UdpListeners = new UDPListener[6];
            _UdpListeners[0] = new UDPListener(_Ports[0]);
            _UdpListeners[1] = new UDPListener(_Ports[1]);
            _UdpListeners[2] = new UDPListener(_Ports[2]);
            _UdpListeners[3] = new UDPListener(_Ports[3]);
            _UdpListeners[4] = new UDPListener(_Ports[4]);
            _UdpListeners[5] = new UDPListener(_Ports[5]);

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
