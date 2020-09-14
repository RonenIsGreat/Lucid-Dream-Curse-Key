using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MongoFetchByTime
{
    static class Constants
    {
        public const string MongoURI = "http://localhost:8080/";
        public const string FilterStart = "?filter={$and:[{\"_date\":{\"$gt\":{\"$date\":";
        public const string FilterMiddle = "}}},{\"_date\":{\"$lt\":{\"$date\":";
        public const string FilterEnd = "}}}]}&pagesize=1000&page=";
    }

    class Program
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            int countRes = 0;

            String encodedAuth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("admin:secret"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", encodedAuth);
            
            Console.WriteLine("Enter Channel Name:");
            string channel = Console.ReadLine();

            Console.WriteLine("Enter First Date:");
            DateTime date1 = new DateTime(DateTime.Parse(Console.ReadLine()).Ticks, DateTimeKind.Utc);
            Console.WriteLine("Enter Second Date:");
            DateTime date2 = new DateTime(DateTime.Parse(Console.ReadLine()).Ticks, DateTimeKind.Utc);

            client.BaseAddress = new Uri(Constants.MongoURI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string query = channel + Constants.FilterStart + ((DateTimeOffset)date1).ToUnixTimeMilliseconds() + Constants.FilterMiddle
                + ((DateTimeOffset)date2).ToUnixTimeMilliseconds() + Constants.FilterEnd;

            string resultsAsString = "";
            int page = 1;

            using (var watch = new SuperWatch())
            {
                for( ; page <= 98 ; page++)
                {
                    try
                    {
                        //Console.WriteLine($"query: {query}");
                        HttpResponseMessage response = await client.GetAsync($"{query}{page.ToString()}");
                        if (response.IsSuccessStatusCode)
                        {
                            resultsAsString = await response.Content.ReadAsStringAsync();
                        }

                        JArray results = JArray.Parse(resultsAsString);

                        //foreach (var res in results)
                        //{

                        //    DateTime resDate = DateTimeOffset.FromUnixTimeMilliseconds((long)res["_date"]["$date"]).DateTime;
                        //    Console.WriteLine($"ID: {res["_id"]["$oid"]}, Date: {resDate.ToString()}");
                        //    ++countRes;
                        //}

                        //Console.WriteLine($"Number of results: {countRes}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            
            Console.ReadLine();
        }
    }
}