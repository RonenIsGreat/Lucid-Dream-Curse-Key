using System;
using System.IO;
using System.Text;
using System.Threading.Tasks.Dataflow;
using GlobalResourses;

namespace SaveStreamHelper
{
    public class SaveStreamHelper
    {
        private BinaryWriter _bytesWriter;
        private BufferBlock<byte[]> _dataBufferBlock;
        private string _saveFileName;
        private ActionBlock<byte[]> _saveToFileBlock;
        private TransformBlock<byte[], byte[]> _transformDataBlock;


        public SaveStreamHelper(string savePathFolder)
        {
            InitializeDataBlocks();

            SavePath = savePathFolder;

            if (Directory.Exists(SavePath)) return;
            Console.WriteLine("Directory does not exist, creating directory...");
            Directory.CreateDirectory(SavePath);
        }

        public string SavePath { get; }

        /// <summary>
        ///     Set file name
        /// </summary>
        /// <param name="fileName"></param>
        public void SetFileName(string fileName)
        {
            _bytesWriter?.Dispose();
            var pathWithFileName = SavePath + '/' + _saveFileName;
            _bytesWriter = new BinaryWriter(File.Open(pathWithFileName, FileMode.OpenOrCreate), Encoding.UTF8);
        }

        private void InitializeDataBlocks()
        {
            _dataBufferBlock = new BufferBlock<byte[]>();
            _transformDataBlock = new TransformBlock<byte[], byte[]>(data => TransformDataCallback(data));
            _saveToFileBlock = new ActionBlock<byte[]>(data => { SaveFileCallback(SavePath, data); });
            _dataBufferBlock.LinkTo(_transformDataBlock);
            _transformDataBlock.LinkTo(_saveToFileBlock);
        }

        private static byte[] TransformDataCallback(byte[] data)
        {
            //TODO: transform data here if needed
            return data;
        }

        private void SaveFileCallback(string savePathFolder, byte[] data)
        {
            ByteArrayToFile(savePathFolder + '/' + _saveFileName, data);
            Console.WriteLine("Saved file: " + _saveFileName);
        }

        private void ByteArrayToFile(string path, byte[] byteArray)
        {
            try
            {
                _bytesWriter.Write(byteArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
            }
        }

        private static string GetBufferType(byte[] serverIdentData)
        {
            var serverIdent = BitConverter.ToUInt16(serverIdentData, 0);
            ChannelNames channel = (ChannelNames) serverIdent;
            var channelName = Enum.GetName(typeof(ChannelNames), channel);
            return channelName != "" ? channelName : null;
        }

        public bool SaveData(byte[] data, string newFileName)
        {
            //Checks wether to change file name
            if (_saveFileName != newFileName)
            {
                _saveFileName = newFileName;
                SetFileName(GetBufferType(data) + "_" + newFileName);
            }

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
    }
}