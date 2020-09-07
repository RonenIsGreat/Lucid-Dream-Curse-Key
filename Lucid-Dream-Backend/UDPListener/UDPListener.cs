using System;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using GlobalResourses;

namespace UDPListener
{
    public class UdpListener
    {
        public delegate void OnDataReceivedDelegate(object sender, StateObject data);

        private readonly IPEndPoint _groupEp;
        private Socket _listener;
        public ChannelDetails Param { get; }
        public BigInteger MessageCount { get; private set; }

        public UdpListener(ChannelDetails port)
        {
            Param = port;

            //Initialize client port
            var clientPort = Param.GetPortNumber();

            //Initialize udp endpoint(server)
            _groupEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), clientPort);
            _listener = new Socket(SocketType.Dgram, ProtocolType.Udp);
            //specefies the number of 1400 bytes messages that have been received
            MessageCount = 0;

            InitSocket();
        } //End UDPListener Constructor

        // Declare the event.
        public event OnDataReceivedDelegate OnDataReceived;

        private void InitSocket()
        {
            _listener.ReceiveTimeout = 1000;
            _listener.ReceiveBufferSize = 1400;
        }

        public void StartListener()
        {
            if (_listener == null)
                _listener = new Socket(SocketType.Dgram, ProtocolType.Udp);
            if (!IsListening())
            {
                try
                {
                    _listener.ExclusiveAddressUse = false;
                    _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _listener.Bind(_groupEp);
                }
                catch (Exception e)
                {
                    OnReceiveError(e);
                }

                BeginReceivingNewData();
            }
        } //End StartListener

        /// <summary>
        ///     Stops the listener temporerly
        /// </summary>
        public void StopListener()
        {
            try
            {
                _listener.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
        }

        /// <summary>
        ///     Hard stops the listner and frees resources
        /// </summary>
        public void HardStop()
        {
            try
            {
                _listener.Shutdown(SocketShutdown.Both);
                _listener.Close();
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
        }

        public bool IsListening()
        {
            return _listener.IsBound;
        }

        private void OnDataRecived(IAsyncResult result)
        {
            var state = (StateObject) result.AsyncState;

            //get current message
            state.bytesCount = _listener.EndReceive(result);
            BeginReceivingNewData();

            if (state.buffer != null)
            {
                OnDataReceived?.Invoke(this, state);
                MessageCount++;
            }
        }

        private void OnReceiveError(Exception e)
        {
            if (e is SocketException socketException)
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
        }

        private void BeginReceivingNewData()
        {
            var state = new StateObject();
            try
            {
                _listener.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None,
                    OnDataRecived, state);
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
    } //End UDPListener
} //End Displaying Listener