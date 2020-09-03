using System;
using GlobalResourses;
using System.Threading;

namespace DisplayingLiveStreams
{
    public class DisplayApp
    {
        Port[] _Ports;
        UDPListener[] _UdpListeners;
        Thread[] _Thraeds;

        public DisplayApp()
        {
            this.Intialize();

        }//DisplayApp Constructor

        private void Intialize()
        {
            //Initializing The Ports
            this._Ports = new Port[6];

            this._Ports[0] = new Port(ChannelNames.CasStave, 25101);
            this._Ports[1] = new Port(ChannelNames.FasTasStave, 25102);
            this._Ports[2] = new Port(ChannelNames.PRSStave, 25103);
            this._Ports[3] = new Port(ChannelNames.CasBeam, 25104);
            this._Ports[4] = new Port(ChannelNames.FasTasBeam, 25105);
            this._Ports[5] = new Port(ChannelNames.IDRSBus, 25106);

            this._UdpListeners = new UDPListener[6];

            for (int i = 0; i < this._UdpListeners.Length; i++)
            {
                this._UdpListeners[i] = new UDPListener(_Ports[i]);
                this._UdpListeners[i].OpenQChannel();

            }//End For

            this._Thraeds = new Thread[6];

            for (int i = 0; i < this._Thraeds.Length; i++)
                this._Thraeds[i] = new Thread(this._UdpListeners[i].StartListener);

        }//End Intialize


        public void ActivatePort(int _PortNumber)
        {
            this._Thraeds[_PortNumber].Start();

        }//End ActivatePort

        public void DeactivatePort(int _PortNumber)
        {
            if (this._Thraeds[_PortNumber].ThreadState != ThreadState.Suspended)
                this._Thraeds[_PortNumber].Suspend();

        }//End DeactivatePort

    }//End Program
}
