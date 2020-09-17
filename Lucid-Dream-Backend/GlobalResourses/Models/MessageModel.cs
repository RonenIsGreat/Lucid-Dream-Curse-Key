using System.Collections.Generic;
using GlobalResourses;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DBManager.Models
{
    [BsonIgnoreExtraElements]
    public class MessageModel
    {
        //Contains message data
        public byte[] Data { get; set; }
        public BsonDateTime TimeStamp { get; set; }
    }
}