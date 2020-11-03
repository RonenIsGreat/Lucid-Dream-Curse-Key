using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DBManager;
using DBManager.Models;
using GlobalResourses;
using MongoDB.Bson;

namespace SaveStream
{
    public class SaveStreamHelper
    {
        //For saving into DB
        private static DatabaseManager _dbManager;
        private readonly ConcurrentExclusiveSchedulerPair _scheduler;

        private BufferBlock<byte[]> _dataBufferBlock;
        private TransformBlock<byte[], MessageModel> _transformByteArrayToWraped;
        private ActionBlock<MessageModel> _saveToFileBlock;


        public SaveStreamHelper()
        {
            _scheduler = new ConcurrentExclusiveSchedulerPair();

            InitializeDataBlocks();

            if (_dbManager == null)
                _dbManager = DatabaseManager.Instance;
        }


        private void InitializeDataBlocks()
        {
            ExecutionDataflowBlockOptions execOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                TaskScheduler = _scheduler.ConcurrentScheduler
            };

            DataflowBlockOptions generalOptions = new DataflowBlockOptions
            {
                TaskScheduler = _scheduler.ConcurrentScheduler
            };

            _dataBufferBlock = new BufferBlock<byte[]>(generalOptions);
            _transformByteArrayToWraped = new TransformBlock<byte[], MessageModel>(TransformDataCallback, execOptions);
            _saveToFileBlock = new ActionBlock<MessageModel>(PushToDb , execOptions);

            _dataBufferBlock.LinkTo(_transformByteArrayToWraped);
            _transformByteArrayToWraped.LinkTo(_saveToFileBlock);
        }

        #region _data Flow Actions

        private void PushToDb(MessageModel message)
        {
            try
            {
                ChannelNames messageType = GetBufferType(message.Data);
                _dbManager.SaveBinaryBased(message, messageType);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
            }
        }

        #endregion

        #region Public Methods

        public bool SaveData(byte[] data)
        {
            try
            {
                _dataBufferBlock.Post(data);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            return false;
        }

        #endregion

        #region Helper Methods

        private static MessageModel TransformDataCallback(byte[] data)
        {
            MessageModel message = new MessageModel
                {Data = data, TimeStamp = new BsonDateTime(DateTime.Now).ToUniversalTime()};
            return message;
        }

        private static ChannelNames GetBufferType(byte[] serverIdentData)
        {
            var serverIdent = BitConverter.ToUInt16(serverIdentData, 0);
            ChannelNames channel = (ChannelNames) serverIdent;
            return channel;
        }

        #endregion
    }
}