using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDataSegmentInsertion
{
    static class Constants
    {
        public const int SegmentSize = 1400;
        public const string DatabaseName = "LD-Database";
        public const int CasNumMessagesInSecond = 9766;
    }
    class Program
    {
        static void Main(string[] args)
        {
            byte[] sampleDataArray = new byte[Constants.SegmentSize];

            var client = new MongoClient("mongodb://127.0.0.1:27017");

            string channel = GetChannel();
            DateTime date = new DateTime(DateTime.Parse("01/01/2015 08:00:00").Ticks, DateTimeKind.Utc);
            //DateTime date = DateTimeOffset.FromUnixTimeSeconds(1420099200).DateTime;
            for (int i=0; i < (9766 * 10); i++)
            {
                //string seconds = i < 10 ? "0" + i.ToString() : i.ToString();
                
                SetUpSampleData(sampleDataArray);

                InsertDocToCollection(client, channel, sampleDataArray, date);

                date = date.AddTicks(1024);

                //Console.WriteLine($"[{i}] Finished inserting to database, Length: {sampleDataArray.Length}");
            }
            
            Console.ReadLine();
        }

        private static string GetChannel()
        {
            Console.WriteLine("Enter name of channel:");
            return Console.ReadLine();
        }

        private static void SetUpSampleData(byte[] byteArray)
        {
            Random rnd = new Random();
            rnd.NextBytes(byteArray);
        }

        private static void InsertDocToCollection(MongoClient client, string collectionName, byte[] byteArray, DateTime date)
        {
            var collection = client.GetDatabase(Constants.DatabaseName).GetCollection<BsonDocument>(collectionName);

            var document = new BsonDocument
            {
                { "_date", date },
                { "data", byteArray }
            };

            collection.InsertOne(document);
        }
    }
}
