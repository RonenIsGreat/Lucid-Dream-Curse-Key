﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace BeamBusFasTas
{
    class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        public class State
        {
            public byte[] buffer = new byte[bufSize];

        }//End State 

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();

        }//End Server

        //Connecting To The Client
        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);

        }//End Client

        //Send Data
        public void Send(byte[] text)
        {
            byte[] data = text;
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
               {
                   State so = (State)ar.AsyncState;
                   int bytes = _socket.EndSend(ar);

               }, state);

        }//End Send

        public void Disconnect()
        {
            _socket.Disconnect(true);
        }

        //Receive Data
        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
                {
                    State so = (State)ar.AsyncState;
                    int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                    _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);

                }, state);

        }//End Receive

    }//End UDPSocket

}//End BeamBusFasTas
