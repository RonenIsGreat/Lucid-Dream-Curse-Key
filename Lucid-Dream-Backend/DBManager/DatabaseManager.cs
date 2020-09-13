using System;
using DBManager.Models;
using GlobalResourses;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBManager
{
    public class DatabaseManager
    {
        private readonly IMongoDatabase _dbClient;

        public DatabaseManager(string connectionString)
        {
            // Get Database for all records
            _dbClient = new MongoClient(connectionString).GetDatabase("records");
            //Create collections for each stream type and indexers for dates
            CreateAllCollections();
        }

        private void CreateAllCollections()
        {
            var channelTypes = Enum.GetNames(typeof(ChannelNames));

            foreach (var channel in channelTypes)
            {
                var collection = _dbClient.GetCollection<MessageModel>(channel);
                CreateIndexers(collection);
            }
        }

        private static async void CreateIndexers(IMongoCollection<MessageModel> collection)
        {
            if (collection == null) return;
            var messageIndexBuilder = Builders<MessageModel>.IndexKeys;
            CreateIndexOptions options = new CreateIndexOptions
            {
                Name = "expireAfterHoursIndex",
                ExpireAfter = TimeSpan.FromHours(1)
            };
            var indexModel =
                new CreateIndexModel<MessageModel>(messageIndexBuilder.Ascending(x => x.Date), options);
            await collection.Indexes.CreateOneAsync(indexModel).ConfigureAwait(false);
        }

        private IMongoCollection<MessageModel> GetCollectionByStreamType(MessageModel message)
        {
            return _dbClient.GetCollection<MessageModel>(Enum.GetName(typeof(ChannelNames), message.ChannelName));
        }

        public void SaveBinaryBased(byte[] content, ChannelNames channelType)
        {
            try
            {
                MessageModel messageModel = new MessageModel
                    {Data = content, Date = BsonDateTime.Create(DateTime.Now), ChannelName = channelType};
                var collectionByType = GetCollectionByStreamType(messageModel);
                collectionByType.InsertOneAsync(messageModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                    \
            }
        }
    }
}