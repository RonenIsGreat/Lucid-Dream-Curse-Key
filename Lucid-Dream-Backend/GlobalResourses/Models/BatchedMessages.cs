using System.Collections.Generic;
using System.Numerics;
using DBManager.Models;
using GlobalResourses;
using MongoDB.Bson;

namespace SaveStreamHelper.Models
{
    public class BatchedMessages
    {
        public BatchedMessages()
        {
            messages = new List<MessageModel>();
        }
        public BsonDateTime _creationDate { get; set; }

        public ChannelNames channelName { get; set; }
        public IList<MessageModel> messages { get; set; }
        public BigInteger messagesNum { get; set; }
    }
}