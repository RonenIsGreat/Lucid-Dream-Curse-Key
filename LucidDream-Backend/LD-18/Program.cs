using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

/* every time the user presses 'Accept', it will be translated to a list of all channels we
 * want to activate.
 * The main program will use direct exchange via RabbitMQ service and will send the checked
 * channels via routing_key "ON" message*/

namespace LD_18
{
    class Controller {

        static private List<ChannelNames> activeList = new List<ChannelNames>(),
                                   nonActiveList = new List<ChannelNames>();

 
        static void Main(string[] args)
        {
            UserInput input = new UserInput();
            while (true)
            {
                WelcomeMessage.WriteMessage();
                var channelsList = input.GetChannelsToActivate(); // input from user 
                // all channels are non active by default
                nonActiveList = Enum.GetValues(typeof(ChannelNames)).Cast<ChannelNames>().ToList();
                foreach (ChannelNames channel in channelsList) {
                    
                    Console.WriteLine($"{channel}"); // TESTING only

                    activeList.Add(channel);                     // update checked channels

                    nonActiveList.Remove(channel);               // update non-checked channels

                    /* send "OFF" to all channels that weren't checked - in nonActiveList
                    *send "ON"  to all channels in activeList Enum.ToString(channel)*/

                }
                //var body = Encoding.UTF8.GetBytes(message); // create a body to send via xchange
            }
            
        }
    }
}
