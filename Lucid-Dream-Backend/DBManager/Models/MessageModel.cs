using GlobalResourses;
using MongoDB.Bson;

namespace DBManager.Models
{
    public class MessageModel
    {
        //Contains message data
        public byte[] Data { get; set; }
        public BsonDateTime Date { get; set; }
        public ChannelNames ChannelName { get; set; }
    }
}