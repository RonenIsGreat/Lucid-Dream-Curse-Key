using System;
using System.Net;
using System.Net.Sockets;
using GlobalResourses;
using System.Timers;
using System.Diagnostics;
using System.Threading;
using System.Numerics;

namespace LiveStreamsDisplay
{
    public class UDPListener
    {
        private Port _Port;
        private UdpClient listener;
        private IPEndPoint groupEP;
        public BigInteger MessageCount { get; private set; }

        public delegate void OnDataReceivedDelegate(object sender, byte[] data);

        // Declare the event.
        public event OnDataReceivedDelegate OnDataReceived;

        public UDPListener(Port port)
        {
            this._Port = port;

            //Initalize client port
            int clientPort = this._Port.GetPortNumber();

            //Initialize udp endpoint(server)
            groupEP = new IPEndPoint(IPAddress.Any, clientPort);
            listener = new UdpClient(groupEP);
            //specefies the number of 1400 bytes messages that have been received
            MessageCount = 0;

            InitSocket();


        }//End UDPListener Constructor

        private void InitSocket()
        {
            listener.Client.ReceiveTimeout = 1000;
            listener.Client.ReceiveBufferSize = 1400;
        }

        public void StartListener()
        {
            if (listener != null)
            {
                BeginReceivingNewData();
            }
        }//End StartListener

        public void StopListener()
        {
            if (listener != null)
            {
                listener.Client.Shutdown(SocketShutdown.Both);
            }
        }

        private void OnDataRecived(IAsyncResult result)
        {
            //get current message
            byte[] received = listener.EndReceive(result, ref groupEP);
            BeginReceivingNewData();

            if (received != null)
            {
                OnDataReceived?.Invoke(this, received);
                MessageCount++;
            }

            //Start receiving next message

            Console.WriteLine("{0} : {1}", this._Port.GetName(), _Port.GetStatus());
            // Console.WriteLine("{0} : {1}", this._Port.GetName(), this._Port.GetS);

        }

        private void OnReceiveError(Exception e)
        {
            if (e is SocketException socketException)
            {
                switch (socketException.ErrorCode)
                {
                    case (int)SocketError.TimedOut:
                        _Port.SetStatus(false);
                        break;
                    case (int)SocketError.Shutdown:
                        _Port.SetStatus(false);
                        break;
                    default:
                        Console.WriteLine(e.Message);
                        break;
                }
            }
        }

        private void BeginReceivingNewData()
        {
            try
            {
                listener.BeginReceive(new AsyncCallback(OnDataRecived), null);

            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
            finally
            {
                if (_Port.GetStatus() == false)
                    _Port.SetStatus(true);
            }
        }

    }//End UDPListener

}//End Displaying Listener
