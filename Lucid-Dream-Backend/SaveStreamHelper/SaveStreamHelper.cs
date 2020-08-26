using System;
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
        private readonly string savePath;
        private TransformBlock<byte[], char[]> transformDataBlock;
        private string saveFileName;
        public SaveStreamHelper()
        {
            //Get the recordings save path for the config file
            this.savePath = ConfigurationManager.AppSettings["save-path"];
            InitilaizeDataBlocks();
        }

        private void InitilaizeDataBlocks()
        {
            dataBufferBlock = new BufferBlock<byte[]>();
            transformDataBlock = new TransformBlock<byte[], char[]>((data) =>
            {
                var encodedData = Encoding.UTF8.GetChars(data);
                //TODO: transform data to saveable data here if needed
                return encodedData;
            });
            saveToFileBlock = new ActionBlock<char[]>( (data) =>
            {
               ByteArrayToFile(savePath + '/' + saveFileName, data);
               var readedBytes = File.ReadAllLines(savePath + '/' + saveFileName, Encoding.UTF8);
                
                Console.WriteLine("Saved file: " + saveFileName);
            });
            dataBufferBlock.LinkTo(transformDataBlock);
            transformDataBlock.LinkTo(saveToFileBlock);
        }

        private bool ByteArrayToFile(string path, char[] byteArray)
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

        public bool saveData(byte[] data, string saveFileName)
        {
            this.saveFileName = saveFileName;
            try
            {
                dataBufferBlock.Post(data);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
            }
            return false;
        }

    }
}
