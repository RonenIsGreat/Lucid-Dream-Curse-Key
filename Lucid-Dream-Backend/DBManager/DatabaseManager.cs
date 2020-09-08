using System;
using DBManager.Models;
using GlobalResourses;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBManager
{
    public class DatabaseManager
    {
        private readonly IMongoCollection<MessageModel> _dbClient;

        public DatabaseManager(string connectionString)
        {
            _dbClient = new MongoClient(connectionString).GetDatabase("records").GetCollection<MessageModel>("records");
            CreateIndexers();
        }

        private async void CreateIndexers()
        {
            var messageIndexBuilder = Builders<MessageModel>.IndexKeys;
            CreateIndexOptions options = new CreateIndexOptions
            {
                Name = "expireAfterHoursIndex",
                ExpireAfter = TimeSpan.FromHours(1)
            };
            var indexModel = new CreateIndexModel<MessageModel>(messageIndexBuilder.Ascending(x => x.Date), options);
            await _dbClient.Indexes.CreateOneAsync(indexModel).ConfigureAwait(false);
        }

        public void SaveBinaryBased(byte[] content, ChannelNames channelType)
        {
            MessageModel messageModel = new MessageModel
                {Data = content, Date = BsonDateTime.Create(DateTime.Now), ChannelName = channelType};
            _dbClient.InsertOneAsync(messageModel);
        }
    }
}