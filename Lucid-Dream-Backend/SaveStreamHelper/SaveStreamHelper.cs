using System;
using System.IO;
using System.Text;
using System.Threading.Tasks.Dataflow;
using GlobalResourses;

namespace SaveStream
{
    public class SaveStreamHelper
    {
        private BinaryWriter _bytesWriter;
        private BufferBlock<byte[]> _dataBufferBlock;
        private string _dateFileName;
        private ActionBlock<byte[]> _saveToFileBlock;
        private TransformBlock<byte[], byte[]> _transformDataBlock;
        private FileStream _currentFileStream;


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
            var pathWithFileName = SavePath + '/' + fileName;
            _currentFileStream = File.Open(pathWithFileName, FileMode.OpenOrCreate);
            _bytesWriter = new BinaryWriter(_currentFileStream, Encoding.UTF8);
        }

        public double GetCurrentFileSize()
        {
            var fileSize = ConvertBytesToMegabytes(_currentFileStream.Length);
            return fileSize;
        }

        private static long ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024) / 1024;
        }


        private void InitializeDataBlocks()
        {
            _dataBufferBlock = new BufferBlock<byte[]>();
            _transformDataBlock = new TransformBlock<byte[], byte[]>(data => TransformDataCallback(data));
            _saveToFileBlock = new ActionBlock<byte[]>(data =>
            {
                SaveFileCallback(data);
            });

            _dataBufferBlock.LinkTo(_transformDataBlock);
            _transformDataBlock.LinkTo(_saveToFileBlock);
        }

        private static byte[] TransformDataCallback(byte[] data)
        {
            //TODO: transform data here if needed
            return data;
        }

        private void SaveFileCallback(byte[] data)
        {
            ByteArrayToFile(data);
        }

        private void ByteArrayToFile(byte[] byteArray)
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
            if (_dateFileName != newFileName)
            {
                _dateFileName = newFileName;
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