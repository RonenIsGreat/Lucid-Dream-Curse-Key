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
        private Consumer _Consumer;
        private UdpClient listener;
        private IPEndPoint groupEP;
        public BigInteger MessageCount { get; private set; }

        public UDPListener(Port port)
        {
            //Initalize client port
            int clientPort = this._Port.GetPortNumber();

            this._Port = port;

            this._Consumer = new Consumer();

            listener = new UdpClient(clientPort);
            //Initialize udp endpoint(server)
            groupEP = new IPEndPoint(IPAddress.Any, clientPort);
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
                listener.Connect(groupEP);
                BeginReceivingNewData();
            }
        }//End StartListener

        public void StopListener()
        {
            if (listener != null)
            {
                listener.Close();
            }
        }

        private void OnDataRecived(IAsyncResult result)
        {
            try
            {
                //get current message
                byte[] received = listener.EndReceive(result, ref groupEP);
                if (received != null)
                {
                    MessageCount++;
                }

                //Start receiving next message
                BeginReceivingNewData();
            }
            catch (Exception e)
            {
                OnReceiveError(e);
            }
            finally
            {
                if (this._Port.GetStatus() == false)
                    this._Port.SetStatus(true);
            }

            Console.WriteLine("{0} : {1}", this._Port.GetName(), _Port.GetStatus());
            // Console.WriteLine("{0} : {1}", this._Port.GetName(), this._Port.GetS);

        }

        private void OnReceiveError(Exception e)
        {
            switch (e)
            {
                case SocketException socketException:
                    if (socketException.ErrorCode == (int)SocketError.TimedOut)
                    {
                        _Port.SetStatus(false);
                    }
                    break;
                default:
                    Console.WriteLine(e.Message);
                    break;
            }
        }

        private void BeginReceivingNewData()
        {
            try
            {
                IAsyncResult result = listener.BeginReceive(new AsyncCallback(OnDataRecived), null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void OpenQChannel()
        {
            this._Consumer.ListenToQueue(this._Port.GetName().ToString());

        }//End OpenQChannel

    }//End UDPListener

}//End Displaying Listener
