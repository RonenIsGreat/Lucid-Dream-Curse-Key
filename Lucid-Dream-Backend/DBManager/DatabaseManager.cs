﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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
        private readonly int _maxMessagesPerDoc;

        private static DatabaseManager _instance;
        public static DatabaseManager Instance
        {
            get
            {
                var dbConnectionUrl = ConfigurationManager.AppSettings["db-url"];
                var maxMessagesPerDoc = ConfigurationManager.AppSettings["maxMessagesPerDoc"];

                if (_instance == null)
                    _instance = new DatabaseManager(dbConnectionUrl, int.Parse(maxMessagesPerDoc));
                return _instance;
            }
        }

        private DatabaseManager(string connectionString, int maxMessagesPerDoc)
        {
            _maxMessagesPerDoc = maxMessagesPerDoc - 1;
            // Get Database for all records
            IMongoClient dbClient = new MongoClient(connectionString);

            _dbDatabase = dbClient.GetDatabase("records");

            //Create collections for each stream type and indexers for dates
            CreateAllCollections();
        }


        #region Public Methods
        public void SaveBinaryBased(MessageModel content, ChannelNames channelType)
        {
            try
            {
                var collectionByType = GetCollectionByStreamType(channelType);
                var newBatchedMessage = getNewBatchedMessages(content, channelType);
                var filter = GetFilterDefinition(newBatchedMessage, _maxMessagesPerDoc);
                var bsonDoc = newBatchedMessage.ToBsonDocument();

                UpdateDefinition<BatchedMessages> updateDefinition = new UpdateDefinitionBuilder<BatchedMessages>().Push(messages => messages.Messages, content);
                foreach (var element in bsonDoc.Elements)
                {
                    //Dont update messages array and number of messages because it's different kind of updates
                    if (element.Name == "Messages" || element.Name == "NumOfMessages")
                        continue;
                    updateDefinition = updateDefinition.SetOnInsert(element.Name, element.Value);
                }

                updateDefinition = updateDefinition.Inc(messages => messages.NumOfMessages, 1);
                collectionByType.UpdateOne(filter, updateDefinition,
                    new UpdateOptions {IsUpsert = true});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        #endregion

        #region Private Methods
        private void CreateAllCollections()
        {
            var channelTypes = Enum.GetNames(typeof(ChannelNames));

            foreach (var channel in channelTypes)
            {
                var collection = _dbDatabase.GetCollection<BatchedMessages>(channel);
                CreateIndexers(collection);
            }
        }

        private void CreateIndexers(IMongoCollection<BatchedMessages> collection)
        {
            if (collection == null) return;
            var messageIndexBuilder = Builders<BatchedMessages>.IndexKeys;
            CreateIndexOptions options = new CreateIndexOptions
            {
                Name = "expireAfterHoursIndex",
                ExpireAfter = TimeSpan.FromHours(1)
            };
            var indexModel =
                new CreateIndexModel<BatchedMessages>(messageIndexBuilder.Descending(x => x.CreationDate), options);
            collection.Indexes.CreateOne(indexModel);

        }

        private IMongoCollection<BatchedMessages> GetCollectionByStreamType(ChannelNames message)
        {
            return _dbDatabase.GetCollection<BatchedMessages>(Enum.GetName(typeof(ChannelNames), message));
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
        #endregion

        #region Helper Methods

        private static FilterDefinition<BatchedMessages> GetFilterDefinition(BatchedMessages messageModel, int maxMessagesPerDoc)
        {
            var builder = Builders<BatchedMessages>.Filter;

            var filter = builder.Eq(message => message.ChannelType, messageModel.ChannelType)
                         &
                         builder.SizeLte(message => message.Messages, maxMessagesPerDoc);


            var dateFilter = builder.Lte(message => message.CreationDate,
                messageModel.CreationDate);

            return filter & dateFilter;
        }
        /**
         * Will try to use this later
         */
        private static FieldDefinition<T>[] GetAllFieldDefinitions<T>(T obj, FieldDefinition<T>[] except = null)
        {
            var fields = new List<FieldDefinition<T>>();
            foreach (var f in obj.GetType().GetFields().Where(f => f.IsPublic))
            {
                var definition = new StringFieldDefinition<T>(f.Name);
                if (!(except ?? Array.Empty<FieldDefinition<T>>()).Contains(definition))
                {
                    fields.Add(new StringFieldDefinition<T>(f.Name));
                }
            }
            return fields.ToArray();
        }
        #endregion

    }
}