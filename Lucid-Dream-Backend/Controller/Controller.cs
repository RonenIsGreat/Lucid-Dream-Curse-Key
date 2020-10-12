using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Controller.RestAPIController;
using GlobalResourses;


/* every time the user presses 'Accept', it will be translated to a list of all channels we
 * want to activate.
 * The main program will use direct exchange via RabbitMQ service and will send the checked
 * channels via routing_key "ON" message*/

namespace Controller
{
    public class Controller
    {


        public static async void RunAsync()
        {

            foreach (string channelName in Enum.GetNames(typeof(ChannelNames)))
            //var input = new UserInput();
                consumersArray[i].ListenToQueue(channelName);
            var server = new RestApiServer(3391);

            await server.StartAsync();

            

        }
    }
}