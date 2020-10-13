using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SaveStream;
using static Tests.StaticVariables;

namespace Tests
{
    [TestFixture]
    public class PhysicalTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var test = 0;
            testData = new byte[1400];

            for (var i = 0; i < testData.Length; i++)
                if (i == 1)
                {
                    testData[i] = Encoding.UTF8.GetBytes("1")[0];
                }
                else
                {
                    var num = Encoding.UTF8.GetBytes(test.ToString());
                    testData[i] = num[0];
                }
        }

        private static SaveStream.SaveStreamHelper _saveStreamHelper;
        private static byte[] testData;
        private string fileName = "TestFileName";
        private const string savePath = "C:\\Recordings\\";

        [Test]
        public void SaveFile_FileExists_True()
        {
            fileName = getStreamType(testData, fileName);
            _saveStreamHelper.SaveData(testData, fileName);
            Assert.True(File.Exists(savePath + fileName), "File does not exist");
        }

        private static string getStreamType(byte[] data, string fileName)
        {
            var fileNameWithType = "";
            var type = Enum.Parse<StreamTypes>(BitConverter.ToInt16(testData, 0).ToString());
            fileNameWithType = Enum.GetName(typeof(StreamTypes), type) + "_" + fileName;
            return fileNameWithType;
        }
    }
}