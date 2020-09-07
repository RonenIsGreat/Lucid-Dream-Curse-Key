using System;
using System.Collections.Generic;
using GlobalResourses;

namespace Controller
{
    internal class UserInput
    {
        public void PrintError()
        {
            Console.WriteLine("argument is not a valid channel, input denied");
        }

        public List<ChannelNames> GetChannelsToActivate()
        {
            var channelsList = new List<ChannelNames>();
            var message = "init";
            while (message != "end")
            {
                message = Console.ReadLine();
                if (message != "end")
                    try
                    {
                        var channel = (ChannelNames) Enum.Parse(typeof(ChannelNames), message ?? string.Empty);
                        if (Enum.IsDefined(typeof(ChannelNames), channel))
                        {
                            channelsList.Add(channel);
                        }
                        else
                        {
                            PrintError();
                            return new List<ChannelNames>();
                        }
                    }
                    catch (ArgumentException)
                    {
                        PrintError();
                        return new List<ChannelNames>();
                    }
            }

            //in the GUI we will translate checkboxes into channels so no input check needed here.
            return channelsList;
        }
    }
}