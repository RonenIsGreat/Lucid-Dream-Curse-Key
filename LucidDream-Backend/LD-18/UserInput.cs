using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD_18
{
    class UserInput
    {
        public List<ChannelNames> GetChannelsToActivate()
        {
            var channelsList = new List<ChannelNames>();
            string message = "init";
            while (message != "end")
            {
                message = Console.ReadLine();
                if (message != "end") {
                    try
                    {
                        ChannelNames channel = (ChannelNames)Enum.Parse(typeof(ChannelNames), message);
                        channelsList.Add(channel);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("argument is not a valid channel, input denied");
                        return new List<ChannelNames>();
                    }
                }
            }
            //in the GUI we will translate checkboxes into channels so no input check needed here.
            return channelsList;
        }
    }
}
