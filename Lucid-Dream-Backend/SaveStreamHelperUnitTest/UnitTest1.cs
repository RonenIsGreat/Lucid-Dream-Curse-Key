using NUnit.Framework;
using SaveStream;
using System.Configuration;
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
        static string savePath;
        [SetUp]
        public void Setup()
        {
            savePath = ConfigurationManager.AppSettings["save-path"];

            int test = 0;
            testData = new byte[1400];

            for (int i = 0; i < testData.Length; i++)
            {
                var num = Encoding.UTF8.GetBytes(test.ToString());
                testData[i] = num[0];
            }
            saveStreamHelper = new SaveStreamHelper(savePath);
        }

        private static void TestTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            saveStreamHelper.saveData(testData, "TestFile");
        }

        [Test]
        public void SaveFile_FileExists_True()
        {
            saveStreamHelper.saveData(testData, testFileName);
        }
    }
}