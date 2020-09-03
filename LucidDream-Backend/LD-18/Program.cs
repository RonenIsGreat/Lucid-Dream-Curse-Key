using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using DisplayingLiveStreams;
using GlobalResourses;

/* every time the user presses 'Accept', it will be translated to a list of all channels we
 * want to activate.
 * The main program will use direct exchange via RabbitMQ service and will send the checked
 * channels via routing_key "ON" message*/

namespace LD_18
{
    class Controller {

        static private List<ChannelNames> activeList = new List<ChannelNames>(),
                                   nonActiveList = new List<ChannelNames>();
        private readonly static string Activate = "ON", Deactivate = "OFF";

 
        static void Main(string[] args)
        {
            DisplayApp LiveStrams = new DisplayApp();
            UserInput input = new UserInput();
            Producer controller = new Producer();

            /*
            Consumer[] consumersArray = new Consumer[6];
            // create distinct routing key for each channel
            int i = 0;
            foreach (string channelName in Enum.GetNames(typeof(ChannelNames)))
            {
                consumersArray[i] = new Consumer();
                consumersArray[i].ListenToQueue(channelName);
                i++;
            }*/
    
            while (true)
            {
                WelcomeMessage.WriteMessage();
                var channelsList = input.GetChannelsToActivate(); // input from user 
                // all channels are non active by default
                activeList.Clear();
                nonActiveList = Enum.GetValues(typeof(ChannelNames)).Cast<ChannelNames>().ToList();
                foreach (var channel in channelsList) {
 
                    activeList.Add(channel);                     // update checked channels

                    nonActiveList.Remove(channel);               // update non-checked channels

                }
                /* 
                Console.WriteLine("ACTIVE");
                activeList.ForEach((ch => Console.WriteLine(ch.ToString())));
                Console.WriteLine("NON ACTIVE");
                nonActiveList.ForEach((ch => Console.WriteLine(ch.ToString())));
                */

                /* send "OFF" to all channels that weren't checked - in nonActiveList
                   *send "ON"  to all channels in activeList Enum.ToString(channel)*/

                foreach (var channelToActivate in activeList) {
                    LiveStrams.ActivatePort((int)channelToActivate);
                    controller.SendMessage(Activate, channelToActivate.ToString());
                }

                foreach (var channelToDeactivate in nonActiveList)
                {
                    LiveStrams.DeactivatePort((int)channelToDeactivate);
                    controller.SendMessage(Deactivate, channelToDeactivate.ToString());
                }
            }

        }
    }
}
