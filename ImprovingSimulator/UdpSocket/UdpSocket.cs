using System.Net;
using System.Net.Sockets;

namespace UdpSocket
{
    public class UDPSocket
    {
        private readonly int bufSize;
        private readonly Socket _socket;
        private readonly State state;

        public UDPSocket(string address, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            bufSize = 8 * 1024;
            state = new State(bufSize);
            _socket.Connect(IPAddress.Parse(address), port);
        }

        public void Disconnect()
        {
            _socket.Disconnect(true);
        }

        public void Send(byte[] text)
        {
            var data = text;
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, ar =>
            {
                var so = (State) ar.AsyncState;
                var bytes = _socket.EndSend(ar);
                //Console.WriteLine("SEND :{0}", bytes);
            }, state);
        } //End Send

        public class State
        {
            public byte[] buffer;

            public State(int bufferSize)
            {
                buffer = new byte[bufferSize];
            }
        } //End State
    } //End UDPSocket
}