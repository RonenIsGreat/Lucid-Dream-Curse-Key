using System;
using System.Collections.Generic;
using System.Linq;
using GlobalResourses;

/* every time the user presses 'Accept', it will be translated to a list of all channels we
 * want to activate.
 * The main program will use direct exchange via RabbitMQ service and will send the checked
 * channels via routing_key "ON" message*/

namespace Controller
{
    public class Controller
    {
        private const string Activate = "ON", Deactivate = "OFF";

        private static readonly List<ChannelNames> activeList = new List<ChannelNames>();

        private static List<ChannelNames> nonActiveList = new List<ChannelNames>();


        public static void Run()
        {
            var input = new UserInput();
            var controller = new Producer();

            /*
            Consumer[] consumersArray = new Consumer[6];
            // create distinct routing key for each channel
            int i = 0;
            foreach (string channelType in Enum.GetNames(typeof(ChannelNames)))
            {
                consumersArray[i] = new Consumer();
                consumersArray[i].ListenToQueue(channelType);
                i++;
            }*/
            while (true)
            {
                WelcomeMessage.WriteMessage();
                var channelsList = input.GetChannelsToActivate(); // input from user 
                // all channels are non active by default
                activeList.Clear();
                nonActiveList = Enum.GetValues(typeof(ChannelNames)).Cast<ChannelNames>().ToList();
                foreach (var channel in channelsList)
                {
                    activeList.Add(channel); // update checked channels

                    nonActiveList.Remove(channel); // update non-checked channels
                }
                /* 
                Console.WriteLine("ACTIVE");
                activeList.ForEach((ch => Console.WriteLine(ch.ToString())));
                Console.WriteLine("NON ACTIVE");
                nonActiveList.ForEach((ch => Console.WriteLine(ch.ToString())));
                */

                /* send "OFF" to all channels that weren't checked - in nonActiveList
                   *send "ON"  to all channels in activeList Enum.ToString(channel)*/

                foreach (var channelToActivate in activeList)
                    controller.SendMessage(Activate, channelToActivate.ToString());

                foreach (var channelToDeactivate in nonActiveList)
                    controller.SendMessage(Deactivate, channelToDeactivate.ToString());
            }
        }
    }
}