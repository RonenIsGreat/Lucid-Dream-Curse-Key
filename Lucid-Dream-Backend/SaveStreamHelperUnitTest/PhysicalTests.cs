using NUnit.Framework;
using SaveStream;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Timers;
using static Tests.StaticVariables;

namespace Tests
{
    [TestFixture]
    public class PhysicalTests
    {
        static SaveStreamHelper saveStreamHelper;
        static byte[] testData;
        string fileName = "TestFileName";
        const string savePath = "C:\\Recordings\\";

        [OneTimeSetUp]
        public void Setup()
        {
            int test = 0;
            testData = new byte[1400];

            for (int i = 0; i < testData.Length; i++)
            {
                if (i ==1)
                {
                    testData[i] = Encoding.UTF8.GetBytes("1")[0];
                }
                else
                {
                    var num = Encoding.UTF8.GetBytes(test.ToString());
                    testData[i] = num[0];
                }
            }
            saveStreamHelper = SaveStreamHelper.Instance;
        }

        [Test]
        public void SaveFile_FileExists_True()
        {
            fileName = getStreamType(testData, fileName);
            saveStreamHelper.SaveData(testData, fileName);
            Assert.True(File.Exists(savePath + fileName), "File does not exist");
        }

        private static string getStreamType(byte[] data, string fileName)
        {
            string fileNameWithType = "";
            StreamTypes type = Enum.Parse<StreamTypes>(BitConverter.ToInt16(testData, 0).ToString());
            fileNameWithType = Enum.GetName(typeof(StreamTypes), type) + "_" + fileName;
            return fileNameWithType;
        }
    }
}