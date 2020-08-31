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
        private ActionBlock<char[]> saveToFileBlock;
        private BufferBlock<byte[]> dataBufferBlock;
        private TransformBlock<byte[], char[]> transformDataBlock;
        private readonly Dictionary<ushort, string> streamTypes;
        private string saveFileName;
        private readonly string savePath;

        public SaveStreamHelper(string savePathFolder)
        {
            this.savePath = savePathFolder;
            streamTypes = new Dictionary<ushort, string>();
            InitializeStreamTypes();

            if (!Directory.Exists(savePath))
            {
                Console.WriteLine("Directory does not exist, creating directory...");
                Directory.CreateDirectory(savePath);
            }
            InitilaizeDataBlocks();
        }

        private void InitializeStreamTypes()
        {
            var streamTypesConfig = ConfigurationManager.GetSection("Enums/streamTypes") as NameValueCollection;
            var streamNames = streamTypesConfig.GetValues(0);
            var streamValues = streamTypesConfig.AllKeys;
            for (int i = 0; i < streamValues.Length; i++)
            {
                streamTypes.Add(ushort.Parse(streamValues[i]), streamNames[i]);
            }
        }

        private void InitilaizeDataBlocks()
        {
            dataBufferBlock = new BufferBlock<byte[]>();
            transformDataBlock = new TransformBlock<byte[], char[]>((data) =>
            {
                return transformDataCallback(data);
            });
            saveToFileBlock = new ActionBlock<char[]>( (data) =>
            {
                saveFileCallback(savePath, data);
            });
            dataBufferBlock.LinkTo(transformDataBlock);
            transformDataBlock.LinkTo(saveToFileBlock);
        }

        private static char[] transformDataCallback(byte[] data)
        {
            var encodedData = Encoding.UTF8.GetChars(data);
            //TODO: transform data here if needed
            return encodedData;
        }

        private void saveFileCallback(string savePathFolder, char[] data)
        {
            ByteArrayToFile(savePathFolder + '/' + saveFileName, data);
            var readedBytes = File.ReadAllLines(savePathFolder + '/' + saveFileName, Encoding.UTF8);

            Console.WriteLine("Saved file: " + saveFileName);
        }

        private static bool ByteArrayToFile(string path, char[] byteArray)
        {
            try
            {
                using (var fs = new StreamWriter(path, true, Encoding.UTF8))
                {
                    fs.WriteLine(byteArray);
                    fs.Flush();
                    fs.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
        private string getBufferType(byte[] serverIdentData)
        {
            ushort serverIdent = BitConverter.ToUInt16(serverIdentData, 0);
            return "";
        }

        private static string getStreamType(byte[] data, string fileName)
        {
            string fileNameWithType = "";
            BitConverter.ToInt16(data, 0).ToString();
            return fileNameWithType;
        }

        public bool SaveData(byte[] data, string saveFileName)
        {
            this.saveFileName = saveFileName;
            try
            {
                dataBufferBlock.Post(data);
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
