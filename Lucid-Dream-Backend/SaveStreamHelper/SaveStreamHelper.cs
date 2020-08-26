using System;
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
        private string saveFileName;
        public SaveStreamHelper(string savePathFolder)
        {
            InitilaizeDataBlocks(savePathFolder);
        }

        private void InitilaizeDataBlocks(string savePathFolder)
        {
            dataBufferBlock = new BufferBlock<byte[]>();
            transformDataBlock = new TransformBlock<byte[], char[]>((data) =>
            {
                return transformDataCallback(data);
            });
            saveToFileBlock = new ActionBlock<char[]>( (data) =>
            {
                saveFileCallback(savePathFolder, data);
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

        public bool SaveData(byte[] data, string saveFileName)
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
