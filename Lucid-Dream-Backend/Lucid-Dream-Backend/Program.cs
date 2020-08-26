using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using SaveStream;

namespace Lucid_Dream_Backend
{
    class Program
    {
        static SaveStreamHelper saveStreamHelper;
        static byte[] testData;
        static void Main(string[] args)
        {
            int test = 0;
            testData = new byte[1400];
            for (int i = 0; i < testData.Length; i++)
            {
                var num = Encoding.UTF8.GetBytes(test.ToString());
                testData[i] = num[0];
            }
            saveStreamHelper = new SaveStreamHelper();

            Timer testTimer = new Timer(1024 / 10)
            {
                AutoReset = true
            };
            testTimer.Elapsed += TestTimer_Elapsed;
            testTimer.Start();

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static  void TestTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            saveStreamHelper.saveData(testData, "TestFile");
        }
    }
}
