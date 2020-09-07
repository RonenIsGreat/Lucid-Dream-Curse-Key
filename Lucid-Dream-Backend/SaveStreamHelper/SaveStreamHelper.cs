using GlobalResourses;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace SaveStream
{
    public class SaveStreamHelper
    {
        private ActionBlock<byte[]> _saveToFileBlock;
        private BufferBlock<byte[]> _dataBufferBlock;
        private TransformBlock<byte[], byte[]> transformDataBlock;
        private string saveFileName;
        public string SavePath { get; private set; }
        private BinaryWriter bytesWriter;


        public SaveStreamHelper(string savePathFolder)
        {
            InitilaizeDataBlocks();

            SavePath = savePathFolder;

            if (!Directory.Exists(SavePath))
            {
                Console.WriteLine("Directory does not exist, creating directory...");
                Directory.CreateDirectory(SavePath);
            }
        }

        /// <summary>
        /// Set file name
        /// </summary>
        /// <param name="path"></param>
        public void SetFileName(string fileName)
        {
            bytesWriter?.Dispose();
            var pathWithFileName = SavePath + '/' + saveFileName;
            bytesWriter = new BinaryWriter(File.Open(pathWithFileName, FileMode.OpenOrCreate), Encoding.UTF8);
        }

        private void InitilaizeDataBlocks()
        {
            _dataBufferBlock = new BufferBlock<byte[]>();
            transformDataBlock = new TransformBlock<byte[], byte[]>((data) =>
            {
                return transformDataCallback(data);
            });
            _saveToFileBlock = new ActionBlock<byte[]>( (data) =>
            {
                saveFileCallback(SavePath, data);
            });
            _dataBufferBlock.LinkTo(transformDataBlock);
            transformDataBlock.LinkTo(_saveToFileBlock);
        }

        private static byte[] transformDataCallback(byte[] data)
        {
            //TODO: transform data here if needed
            return data;
        }

        private void saveFileCallback(string savePathFolder, byte[] data)
        {
            ByteArrayToFile(savePathFolder + '/' + saveFileName, data);
            Console.WriteLine("Saved file: " + saveFileName);
        }

        private bool ByteArrayToFile(string path, byte[] byteArray)
        {
            try
            {
                bytesWriter.Write(byteArray);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
        private string GetBufferType(byte[] serverIdentData)
        {
            ushort serverIdent = BitConverter.ToUInt16(serverIdentData, 0);
            ChannelNames channel = (ChannelNames) serverIdent;
            var channelName = Enum.GetName(typeof(ChannelNames), channel);
            if (channelName != "")
                return channelName;
            return null;
        }

        public bool SaveData(byte[] data, string newFileName)
        {
            //Checks wether to change file name
            if(this.saveFileName != newFileName)
            {
                this.saveFileName = newFileName;
                SetFileName(GetBufferType(data) + "_" + newFileName);
            }
            try
            {
                _dataBufferBlock.Post(data);
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
            }
            return false;
        }

    }
}