using System;
using System.Configuration;
using Controller;
using GlobalResourses;
using SaveStream;

namespace Lucid_Dream_Backend
{
    internal class Program
    {
        private static SaveStreamHelper saveStreamHelper;

        private static void Main(string[] args)
        {
            //Initlize and get the save stream helper instance
            var savePath = ConfigurationManager.AppSettings["save-path"];
            saveStreamHelper = new SaveStreamHelper(savePath);

            //Initializing The Ports
            var _Ports = new ChannelDetails[6];

            _Ports[0] = new ChannelDetails(ChannelNames.CasStave, 25101);
            _Ports[1] = new ChannelDetails(ChannelNames.FasTasStave, 25102);
            _Ports[2] = new ChannelDetails(ChannelNames.PRSStave, 25103);
            _Ports[3] = new ChannelDetails(ChannelNames.CasBeam, 25104);
            _Ports[4] = new ChannelDetails(ChannelNames.FasTasBeam, 25105);
            _Ports[5] = new ChannelDetails(ChannelNames.IDRSBus, 25106);

            UDPListener[] _UdpListeners = new UDPListener[6];
            Consumer[] _Consumers = new Consumer[6];

            for (var i = 0; i < _UdpListeners.Length; i++)
            {
                _UdpListeners[i] = new UDPListener(_Ports[i]);
                _UdpListeners[i].OnDataReceived += onDataReceived;
                _Consumers[i] = new Consumer(_UdpListeners[i]);
                _Consumers[i].ListenToQueue();
            } //End For


            Console.WriteLine("Live Streams :");
            Controller.Controller.Run();
        } //End Main

        private static void onDataReceived(object sender, StateObject data)
        {
            UDPListener currentListener = (UDPListener) sender;

            //This date format can be saved as file name
            var dateAsString = DateTime.Now.ToString("yyyy-dd-M--HH-mm");
            var succeeded = saveStreamHelper.SaveData(data.buffer, dateAsString);
            if (!succeeded) Console.WriteLine("Failed to save message");
            //Data goes in here
        }
    } //End Program
} //End Lucid_Dream_Backend