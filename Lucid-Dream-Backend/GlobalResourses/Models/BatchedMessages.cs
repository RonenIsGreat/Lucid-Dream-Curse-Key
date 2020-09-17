using System;
using System.Collections.Generic;
using System.Numerics;
using DBManager.Models;
using GlobalResourses;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SaveStreamHelper.Models
{
    [BsonIgnoreExtraElements]
    public class BatchedMessages
    {
        public BatchedMessages(ChannelNames channelType)
        {
            this.ChannelType = Enum.GetName(typeof(ChannelNames), channelType);
            Messages = new List<MessageModel>();
        }

        public BsonDateTime CreationDate { get; set; }

        public string ChannelType { get; set; }
        public IList<MessageModel> Messages { get; set; }
        public int NumOfMessages { get; set; }
    }
}