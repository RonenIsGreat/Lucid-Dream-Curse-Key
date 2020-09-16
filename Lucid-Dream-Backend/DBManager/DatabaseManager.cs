using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DBManager.Models;
using GlobalResourses;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using SaveStreamHelper.Models;

namespace DBManager
{
    public class DatabaseManager
    {
        private readonly IMongoDatabase _dbDatabase;
        private readonly IMongoClient _dbClient;
        private readonly BigInteger maxMessagesPerDoc;
        //Limit number of threads to be used for queueing db requests
        private readonly Semaphore openConnectionSemaphore;

        public DatabaseManager(string connectionString, BigInteger maxMessagesPerDoc)
        {
            this.maxMessagesPerDoc = maxMessagesPerDoc;
            // Get Database for all records
            _dbClient = new MongoClient(connectionString);

            _dbDatabase = _dbClient.GetDatabase("records");

            openConnectionSemaphore = new Semaphore(_dbClient.Settings.MaxConnectionPoolSize,
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
            var dbRequest = collection.Indexes.CreateOneAsync(indexModel).ConfigureAwait(false);
            await AddDbRequest<string>(openConnectionSemaphore, dbRequest);
        }

        private static async Task<T> AddDbRequest<T>(Semaphore semaphore, ConfiguredTaskAwaitable<string> task)
        {
            //TODO: fix this
            semaphore.WaitOne();
            try
            {
                T result = await task;
                return result;
            }
            finally
            {
                semaphore.Release();
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

                await collectionByType.FindOneAndUpdateAsync(filter, appeandAction,
                    new FindOneAndUpdateOptions<BatchedMessages> {IsUpsert = true});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void ContinuationAction(object state)
        {
            //TODO: remove test
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

        private static FilterDefinition<BatchedMessages> GetFilterDefinition(BatchedMessages messageModel, BigInteger maxMessagesPerDoc)
        {
            var builder = Builders<BatchedMessages>.Filter;

            var filter = builder.Eq(messages => messages.ChannelType, messageModel.ChannelType) 
                         & 
                         builder.Lt(messages => messages.NumOfMessages, maxMessagesPerDoc);
            return filter;
        }

    }
}