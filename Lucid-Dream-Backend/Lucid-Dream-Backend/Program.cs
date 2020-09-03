using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GlobalResourses;
using SaveStream;
using System.Configuration;
using Controller;

namespace Lucid_Dream_Backend
{
    class Program
    {
        static SaveStreamHelper saveStreamHelper;
        static void Main(string[] args)
        {
            //Initlize and get the save stream helper instance
            var savePath = ConfigurationManager.AppSettings["save-path"];
            saveStreamHelper = SaveStreamHelper.Instance;
            saveStreamHelper.InitSaveStreamHelper(savePath);

            //Initializing The Ports
            Port[] _Ports = new Port[6];

            _Ports[0] = new Port(ChannelNames.CasStave, 25101);
            _Ports[1] = new Port(ChannelNames.FasTasStave, 25102);
            _Ports[2] = new Port(ChannelNames.PRSStave, 25103);
            _Ports[3] = new Port(ChannelNames.CasBeam, 25104);
            _Ports[4] = new Port(ChannelNames.FasTasBeam, 25105);
            _Ports[5] = new Port(ChannelNames.IDRSBus, 25106);

            UDPListener[] _UdpListeners = new UDPListener[6];
            Consumer[] _Consumers = new Consumer[6];

            for (int i = 0; i < _UdpListeners.Length; i++)
            {
                _UdpListeners[i] = new UDPListener(_Ports[i]);
                _UdpListeners[i].OnDataReceived += onDataReceived;
                _Consumers[i] = new Consumer(_UdpListeners[i]);
                _Consumers[i].ListenToQueue();

            }//End For

          
            Console.WriteLine("Live Streams :");
            Controller.Controller.Run();

        }//End Main

        private static void onDataReceived(object sender, byte[] data)
        {
            //This date format can be saved as file name
            var dateAsString = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            bool succeeded = saveStreamHelper.SaveData(data, dateAsString);
            if (!succeeded)
            {
                Console.WriteLine("Failed to save message");
            }
            else
            {
                //Also write data in console
                var dataAsUtf8 = Encoding.UTF8.GetString(data);
            }
            //Data goes in here
        }
    }//End Program

}//End Lucid_Dream_Backend
