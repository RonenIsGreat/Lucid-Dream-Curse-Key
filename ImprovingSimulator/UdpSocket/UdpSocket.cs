using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpSocket
{
    public class UDPSocket
    {
        private Socket _socket;
        private readonly int bufSize;
        private State state;

        public UDPSocket(string address, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            bufSize = 8 * 1024;
            state = new State(bufSize);
            _socket.Connect(IPAddress.Parse(address), port);
        }

        public class State
        {
            public byte[] buffer;

            public State(int bufferSize)
            {
                buffer = new byte[bufferSize];
            }

        }//End State

        public void Disconnect()
        {
            _socket.Disconnect(true);
        }

        public void Send(byte[] text)
        {
            byte[] data = text;
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                //Console.WriteLine("SEND :{0}", bytes);
            }, state);

        }//End Send

    }//End UDPSocket
}
