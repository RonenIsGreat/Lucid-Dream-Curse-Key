using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace LD_18
{
    class Controller
    {
        static void Main(string[] args)
        {
            UserInput input = new UserInput();
            while (true)
            {
                WelcomeMessage.WriteMessage();
                var channelsList = input.GetChannelsToActivate(); // input from user 
                foreach (ChannelNames channel in channelsList) {
                    //send "ON"  to routing_key "channel" by using Enum.ToString(channel)
                    Console.WriteLine($"{channel}"); // TESTING only
                }
                //var body = Encoding.UTF8.GetBytes(message); // create a body to send via xchange
            }
            
        }
    }
}
