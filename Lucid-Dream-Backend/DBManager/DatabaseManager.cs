using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DBManager.Models;
using GlobalResourses;
using MongoDB.Bson;
using MongoDB.Driver;
using SaveStreamHelper.Models;

namespace DBManager
{
    public class DatabaseManager
    {
        private readonly IMongoDatabase _dbDatabase;
        private readonly IMongoClient _dbClient;
        private readonly int maxMessagesPerDoc;
        //Limit number of threads to be used for queueing db requests
        private readonly SemaphoreSlim openConnectionSemaphore;

        public DatabaseManager(string connectionString, int maxMessagesPerDoc)
        {
            this.maxMessagesPerDoc = maxMessagesPerDoc;
            // Get Database for all records
            _dbClient = new MongoClient(connectionString);

            _dbDatabase = _dbClient.GetDatabase("records");

            openConnectionSemaphore = new SemaphoreSlim(_dbClient.Settings.MaxConnectionPoolSize,
                _dbClient.Settings.MaxConnectionPoolSize);

            //Create collections for each stream type and indexers for dates
            CreateAllCollections();
        }

        private void CreateAllCollections()
        {
            var channelTypes = Enum.GetNames(typeof(ChannelNames));

            foreach (var channel in channelTypes)
            {
                var collection = _dbDatabase.GetCollection<BatchedMessages>(channel);
                CreateIndexers(collection);
            }
        }

        private async void CreateIndexers(IMongoCollection<BatchedMessages> collection)
        {
            if (collection == null) return;
            var messageIndexBuilder = Builders<BatchedMessages>.IndexKeys;
            CreateIndexOptions options = new CreateIndexOptions
            {
                Name = "expireAfterHoursIndex",
                ExpireAfter = TimeSpan.FromHours(1)
            };
            var indexModel =
                new CreateIndexModel<BatchedMessages>(messageIndexBuilder.Ascending(x => x.CreationDate), options);
            var dbRequest = collection.Indexes.CreateOneAsync(indexModel);

            await AddDbRequest(dbRequest);
        }

        private async Task AddDbRequest<T>(Task<T> task)
        {
            await openConnectionSemaphore.WaitAsync();
            try
            {
                await task;
            }
            catch (Exception e)
            {
                await Task.FromException<T>(e);
            }
            finally
            {
                openConnectionSemaphore.Release();
            }
        }

        private IMongoCollection<BatchedMessages> GetCollectionByStreamType(ChannelNames message)
        {
            return _dbDatabase.GetCollection<BatchedMessages>(Enum.GetName(typeof(ChannelNames), message));
        }

        public async void SaveBinaryBased(MessageModel content, ChannelNames channelType)
        {
            try
            {
                var collectionByType = GetCollectionByStreamType(channelType);
                var newBatchedMessage = getNewBatchedMessages(content, channelType);
                var filter = GetFilterDefinition(newBatchedMessage, maxMessagesPerDoc);

                var appeandAction = Builders<BatchedMessages>.Update
                    .Push(messages => messages.Messages, content)
                    .Inc(messages => messages.NumOfMessages, 1);

                var task = collectionByType.FindOneAndUpdateAsync(filter, appeandAction,
                    new FindOneAndUpdateOptions<BatchedMessages> { IsUpsert = true });

                await AddDbRequest(task);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private BatchedMessages getNewBatchedMessages(MessageModel firstMessage, ChannelNames channelType)
        {
            BatchedMessages newBatchedMessage = new BatchedMessages(channelType)
            {
                Messages = new List<MessageModel>(),
                CreationDate = BsonDateTime.Create(DateTime.Now.ToUniversalTime()),
                NumOfMessages = 0
            };
            newBatchedMessage.Messages.Add(firstMessage);
            return newBatchedMessage;
        }

        private static FilterDefinition<BatchedMessages> GetFilterDefinition(BatchedMessages messageModel, int maxMessagesPerDoc)
        {
            var builder = Builders<BatchedMessages>.Filter;

            var filter = builder.Eq(messages => messages.ChannelType, messageModel.ChannelType)
                         &
                         builder.Lt(messages => messages.NumOfMessages, maxMessagesPerDoc);

            var dateFilter = builder.Lte(messages => messages.CreationDate, messageModel.CreationDate);

            return filter & dateFilter;
        }

    }
}