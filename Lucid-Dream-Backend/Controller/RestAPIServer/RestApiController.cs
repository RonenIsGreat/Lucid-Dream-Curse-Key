﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using GlobalResourses;
using System.Web.Http.Cors;

namespace Controller.RestAPIController
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class ChannelController : ApiController
    {
        private const string Activate = "ON", Deactivate = "OFF";

        private static readonly List<ChannelNames> activeList = new List<ChannelNames>();

        private static List<ChannelNames> nonActiveList = new List<ChannelNames>();

        Producer controller = new Producer();

        [HttpPost]

        public IHttpActionResult PostActiveList(string[] checkedChannels)
        {
            activeList.Clear();
            nonActiveList = Enum.GetValues(typeof(ChannelNames)).Cast<ChannelNames>().ToList();
            foreach (var channel in checkedChannels)
            {
                activeList.Add((ChannelNames) Enum.Parse(typeof(ChannelNames),channel)); // update checked channels

                nonActiveList.Remove((ChannelNames) Enum.Parse(typeof(ChannelNames),channel)); // update non-checked channels
            }
            

            /* send "OFF" to all channels that weren't checked - in nonActiveList
               *send "ON"  to all channels in activeList Enum.ToString(channel)*/

            foreach (var channelToActivate in activeList)
                Producer.SendMessage(Activate, channelToActivate.ToString());

            foreach (var channelToDeactivate in nonActiveList)
                Producer.SendMessage(Deactivate, channelToDeactivate.ToString());

            return Ok();
        }
    }
}
     
   