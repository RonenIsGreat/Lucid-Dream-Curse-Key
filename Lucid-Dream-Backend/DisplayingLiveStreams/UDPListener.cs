using System;
using System.Net;
using System.Net.Sockets;
using GlobalResourses;
using System.Timers;
using System.Diagnostics;


namespace DisplayingLiveStreams
{
    public class UDPListener
    {
        private Port _Port;
        private Stopwatch _StopWatch;
        int count = 0;

        public UDPListener(Port port)
        {
            this._Port = port;

        }//End UDPListener Constructor

        public void StartListener()
        {
            int ListenPort = this._Port.GetPortNumber();
            UdpClient listener = new UdpClient(ListenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, ListenPort);
            Timer RecieveTimer = new Timer(1000);
            RecieveTimer.Elapsed += OnTimedEvent;
           
            _StopWatch = new Stopwatch();
            RecieveTimer.Start();
            try
            {
                while (true)
                {
                    _StopWatch.Start();
                    byte[] bytes = listener.Receive(ref groupEP);
                    count++;
                    _StopWatch.Reset();
                    


                }//End While

            }//End Try

            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
                RecieveTimer.Stop();
                _StopWatch.Stop();
                this._Port.SetStatus(false);
            }

        }//End StartListener

        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            double Mili = _StopWatch.ElapsedMilliseconds;

            if (Mili > 1000)
                this._Port.SetStatus(false);
            else
                if (this._Port.GetStatus() == false)
                this._Port.SetStatus(true);

            Console.WriteLine("{0} : {1}", this._Port.GetName(), this._Port.GetStatus());
           // Console.WriteLine("{0} : {1}", this._Port.GetName(), this._Port.GetS);
            count = 0;
            

        }//End OnTimeEvent

    }//End UDPListener

}//End Displaying Listener
