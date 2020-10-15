using System;
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

        private readonly IPEndPoint _localEndPoint;
        private IPEndPoint _remoteEndPoint;
        private Socket _socket;

        public UdpListener(ChannelDetails port)
        {
            Param = port;

            //Initialize client port
            var clientPort = Param.GetPortNumber();

            //TODO: add this to config instead of constructor
            _localEndPoint = new IPEndPoint(IPAddress.Loopback, clientPort+1);

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

            _socket.Bind(_localEndPoint);
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

                _socket.Connect(_remoteEndPoint);

                statusSender.SendStatus($"{channelName} active");
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }

            BeginReceivingNewData();
        } //End StartListener

        /// <summary>
        ///     Stops the listener temporally
        /// </summary>
        public void StopListener()
        {
            try
            {
                var channelName = Enum.GetName(typeof(ChannelNames), Param.GetName());
                var statusSender = new ChannelStatusSender();
                _socket.Disconnect(true);
                statusSender.SendStatus($"{channelName} inactive");
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
        }

        /// <summary>
        ///     Hard stops the listener and frees resources
        /// </summary>
        public void HardStop()
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
            return _socket.Connected;
        }

        #endregion

        #region Callbacks

        private void OnDataReceived(IAsyncResult result)
        {
            var state = (StateObject) result.AsyncState;

            //get current message
            state.bytesCount = _socket.EndReceive(result);
            BeginReceivingNewData();

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