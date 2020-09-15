using System;
using System.Collections.Generic;
using System.Numerics;
using DBManager.Models;
using GlobalResourses;
using MongoDB.Bson;
using MongoDB.Driver;
using SaveStreamHelper.Models;

namespace DBManager
{
    public class DatabaseManager
    {
        private readonly IMongoDatabase _dbClient;
        private readonly BigInteger maxMessagesPerDoc;

        public DatabaseManager(string connectionString, BigInteger maxMessagesPerDoc)
        {
            this.maxMessagesPerDoc = maxMessagesPerDoc;
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
                var collection = _dbClient.GetCollection<BatchedMessages>(channel);
                CreateIndexers(collection);
            }
        }

        private static async void CreateIndexers(IMongoCollection<BatchedMessages> collection)
        {
            if (collection == null) return;
            var messageIndexBuilder = Builders<BatchedMessages>.IndexKeys;
            CreateIndexOptions options = new CreateIndexOptions
            {
                Name = "expireAfterHoursIndex",
                ExpireAfter = TimeSpan.FromHours(1)
            };
            var indexModel =
                new CreateIndexModel<BatchedMessages>(messageIndexBuilder.Ascending(x => x._creationDate), options);
            await collection.Indexes.CreateOneAsync(indexModel).ConfigureAwait(false);
        }

        private IMongoCollection<BatchedMessages> GetCollectionByStreamType(BatchedMessages message)
        {
            return _dbClient.GetCollection<BatchedMessages>(Enum.GetName(typeof(ChannelNames), message.channelName));
        }

        public void SaveBinaryBased(IList<MessageModel> content, ChannelNames channelType)
        {
            try
            {
                BatchedMessages messageModel = new BatchedMessages
                { messages = content, _creationDate = BsonDateTime.Create(DateTime.Now.ToUniversalTime()), channelName = channelType};
                var collectionByType = GetCollectionByStreamType(messageModel);

                var filter = GetFilterDefinition(messageModel, maxMessagesPerDoc);
                var appeandAction = Builders<BatchedMessages>.Update
                    .PushEach(messages => messages.messages, content);

                collectionByType.FindOneAndUpdateAsync(filter, appeandAction, new FindOneAndUpdateOptions<BatchedMessages, BatchedMessages>{IsUpsert = true});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static FilterDefinition<BatchedMessages> GetFilterDefinition(BatchedMessages messageModel, BigInteger maxMessagesPerDoc)
        {
            var builder = Builders<BatchedMessages>.Filter;

            var filter = builder.Eq(model => model._creationDate == messageModel._creationDate, true)
                         &
                builder.Lt(messages => messages.messagesNum, maxMessagesPerDoc);
            return filter;
        }

    }
}