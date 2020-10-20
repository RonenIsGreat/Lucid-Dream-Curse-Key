﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using GlobalResourses;

namespace UDPListener
{
    public class UdpListener
    {
        public delegate void OnDataReceivedDelegate(object sender, StateObject data);

        private readonly IPEndPoint _remoteEndPoint;
        private Socket _socket;

        public UdpListener(ChannelDetails port)
        {
            Param = port;

            //Initialize client port
            var clientPort = Param.GetPortNumber();

            //TODO: add this to config instead of constructor
            _remoteEndPoint = new IPEndPoint(IPAddress.Loopback, clientPort);

            MessageCount = 0;

            InitSocket();
        } //End UDPListener Constructor

        public ChannelDetails Param { get; }
        public int MessageCount { get; private set; }

        // Declare the event.
        public event OnDataReceivedDelegate DataReceivedDelegate;

        private void InitSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);

            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 1400);
        }

        #region Helper Methods

        private void OnReceiveError(Exception e)
        {
            var channelName = Enum.GetName(typeof(ChannelNames), Param.GetName());
            var statusSender = new ChannelStatusSender();
            if (!(e is SocketException socketException)) return;
            switch (socketException.ErrorCode)
            {
                case (int) SocketError.TimedOut:
                    //Request timed out
                    Param.SetStatus(false);
                    break;
                case (int) SocketError.Shutdown:
                    //Socket has been closed
                    Param.SetStatus(false);
                    break;
                default:
                    Console.WriteLine(e.Message);
                    break;
            }
            statusSender.SendStatus($"{channelName} inactive");
        }

        #endregion

        #region Public Methods

        public void StartListener()
        {
            if(_socket == null) InitSocket();
            //If already listening return like nothing happened
            if (IsListening())
            {
                var channelName = Enum.GetName(typeof(ChannelNames), Param.GetName());
                var statusSender = new ChannelStatusSender();
                statusSender.SendStatus($"{channelName} active");
                return;
            }
            try
            {
                var channelName = Enum.GetName(typeof(ChannelNames), Param.GetName());
                var statusSender = new ChannelStatusSender();
                _socket.Bind(_remoteEndPoint);
                statusSender.SendStatus($"{channelName} active");
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }

            BeginReceivingNewData();
        } //End StartListener

        /// <summary>
        ///     Stops the listener
        /// </summary>
        public void StopListener()
        {
            try
            {
                var channelName = Enum.GetName(typeof(ChannelNames), Param.GetName());
                var statusSender = new ChannelStatusSender();
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                statusSender.SendStatus($"{channelName} inactive");
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
        }

        public bool IsListening()
        {
            return _socket.IsBound;
        }

        #endregion

        #region Callbacks

        private void OnDataReceived(IAsyncResult result)
        {
            var state = (StateObject) result.AsyncState;

            //get current message
            try
            {
                state.bytesCount = _socket.EndReceive(result);
                BeginReceivingNewData();
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }

            if (state.buffer == null) return;
            DataReceivedDelegate?.Invoke(this, state);
            MessageCount++;
        }

        private void BeginReceivingNewData()
        {
            var state = new StateObject();
            try
            {
                _socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None,
                    OnDataReceived, state);
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
            finally
            {
                if (Param.GetStatus() == false)
                    Param.SetStatus(true);
            }
        }

        #endregion
    } //End UDPListener
} //End Displaying Listener