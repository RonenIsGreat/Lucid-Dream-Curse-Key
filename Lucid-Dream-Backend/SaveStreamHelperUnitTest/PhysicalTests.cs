using NUnit.Framework;
using SaveStream;
using System.Configuration;
using System.IO;
using System.Text;
using System.Timers;

namespace Tests
{
    [TestFixture]
    public class PhysicalTests
    {
        static SaveStreamHelper saveStreamHelper;
        static byte[] testData;
        const string testFileName = "TestFileName";
        const string savePath = "C:\\Recordings\\";

        [SetUp]
        public void Setup()
        {
            int test = 0;
            testData = new byte[1400];

            for (int i = 0; i < testData.Length; i++)
            {
                var num = Encoding.UTF8.GetBytes(test.ToString());
                testData[i] = num[0];
            }
            saveStreamHelper = new SaveStreamHelper(savePath);
        }

        [Test]
        public void SaveFile_FileExists_True()
        {
            saveStreamHelper.SaveData(testData, testFileName);
            Assert.True(File.Exists(savePath + testFileName), "File does not exist");
        }
    }
}