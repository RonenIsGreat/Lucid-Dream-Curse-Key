using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DBManager;
using DBManager.Models;
using GlobalResourses;
using SaveStreamHelper.Models;

namespace SaveStream
{
    public class SaveStreamHelper
    {
        //For saving into DB
        private static DatabaseManager _dbManager;
        private readonly ConcurrentExclusiveSchedulerPair _scheduler;

        private Dictionary<ChannelNames, ulong> NumberOfMessagesByChannelTypeMap;

        private BufferBlock<byte[]> _dataBufferBlock;
        private TransformBlock<byte[], BatchedMessages> _transformByteArrayToWraped;
        private ActionBlock<IList<BatchedMessages>> _saveToFileBlock;


        public SaveStreamHelper(string dbConnectionUrl)
        {
            _scheduler = new ConcurrentExclusiveSchedulerPair();
            NumberOfMessagesByChannelTypeMap = new Dictionary<ChannelNames, ulong>();

            InitializeDataBlocks();

            if (_dbManager == null)
                _dbManager = new DatabaseManager(dbConnectionUrl);
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

            _transformByteArrayToWraped = new TransformBlock<byte[], BatchedMessages>(data => TransformDataCallback(data), execOptions);
            _dataBufferBlock = new BufferBlock<byte[]>(generalOptions);
            _saveToFileBlock = new ActionBlock<IList<BatchedMessages>>(data => { PushToDb(data); }, execOptions);

            _dataBufferBlock.LinkTo(_transformByteArrayToWraped);
            _transformByteArrayToWraped.LinkTo(_saveToFileBlock);
        }

        #region Data Flow Actions

        private static void PushToDb(IList<BatchedMessages> multipleByteArray)
        {
            try
            {
                foreach (BatchedMessages batchedMessages in multipleByteArray)
                {
                    _dbManager.SaveBinaryBased(batchedMessages.messages, batchedMessages.channelName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
            }
        }

        #endregion

        #region Public Methods

        public bool SaveData(byte[] data, string newFileName)
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

        private static byte[] TransformDataCallback(byte[] data)
        {
            return data;
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