using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DBManager;
using GlobalResourses;

namespace SaveStream
{
    public class SaveStreamHelper
    {
        //For saving into DB
        private static DatabaseManager _dbManager;
        private readonly ConcurrentExclusiveSchedulerPair _scheduler;

        private BufferBlock<byte[]> _dataBufferBlock;
        private ActionBlock<byte[]> _saveToFileBlock;
        private TransformBlock<byte[], byte[]> _transformDataBlock;


        public SaveStreamHelper(string dbConnectionUrl)
        {
            _scheduler = new ConcurrentExclusiveSchedulerPair();

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

            _dataBufferBlock = new BufferBlock<byte[]>(generalOptions);
            _transformDataBlock = new TransformBlock<byte[], byte[]>(data => TransformDataCallback(data), execOptions);
            _saveToFileBlock = new ActionBlock<byte[]>(data => { PushToDb(data); }, execOptions);

            _dataBufferBlock.LinkTo(_transformDataBlock);
            _transformDataBlock.LinkTo(_saveToFileBlock);
        }

        #region Data Flow Actions

        private static void PushToDb(byte[] byteArray)
        {
            try
            {
                ChannelNames channelType = GetBufferType(byteArray);
                _dbManager.SaveBinaryBased(byteArray, channelType);
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
            //TODO: transform data here if needed
            return data;
        }

        private static ChannelNames GetBufferType(byte[] serverIdentData)
        {
            var serverIdent = BitConverter.ToUInt16(serverIdentData, 0);
            ChannelNames channel = (ChannelNames)(serverIdent - 1);
            return channel;
        }

        #endregion
    }
}