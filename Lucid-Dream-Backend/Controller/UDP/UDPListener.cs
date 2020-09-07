using System;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using GlobalResourses;

namespace Controller
{
    public class UDPListener
    {
        public delegate void OnDataReceivedDelegate(object sender, StateObject data);

        private readonly IPEndPoint groupEP;
        private Socket listener;

        public UDPListener(ChannelDetails port)
        {
            _Param = port;

            //Initalize client port
            var clientPort = _Param.GetPortNumber();

            //Initialize udp endpoint(server)
            groupEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), clientPort);
            listener = new Socket(SocketType.Dgram, ProtocolType.Udp);
            //specefies the number of 1400 bytes messages that have been received
            MessageCount = 0;

            InitSocket();
        } //End UDPListener Constructor

        public ChannelDetails _Param { get; }
        public BigInteger MessageCount { get; private set; }

        // Declare the event.
        public event OnDataReceivedDelegate OnDataReceived;

        private void InitSocket()
        {
            listener.ReceiveTimeout = 1000;
            listener.ReceiveBufferSize = 1400;
        }

        public void StartListener()
        {
            if (listener == null)
                listener = new Socket(SocketType.Dgram, ProtocolType.Udp);
            if (!IsListening())
            {
                try
                {
                    listener.ExclusiveAddressUse = false;
                    listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    listener.Bind(groupEP);
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
                listener.Shutdown(SocketShutdown.Both);
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
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
        }

        public bool IsListening()
        {
            return listener.IsBound;
        }

        private void OnDataRecived(IAsyncResult result)
        {
            var state = (StateObject) result.AsyncState;

            //get current message
            state.bytesCount = listener.EndReceive(result);
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
                        _Param.SetStatus(false);
                        break;
                    case (int) SocketError.Shutdown:
                        //Socket has been closed
                        _Param.SetStatus(false);
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
                listener.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None,
                    OnDataRecived, state);
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
            finally
            {
                if (_Param.GetStatus() == false)
                    _Param.SetStatus(true);
            }
        }
    } //End UDPListener
} //End Displaying Listener