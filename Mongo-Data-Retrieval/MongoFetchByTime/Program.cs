using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Specialized;

namespace MongoFetchByTime
{
    static class Constants
    {
        // http://localhost:8080/CasStave?filter={"Messages": {"$elemMatch": { "TimeStamp": { "$gt": {"$date":1603209660000} }, "TimeStamp": { "$lt": {"$date":1603209660000} } } } }
        public const string MongoURI = "http://localhost:8080/";
        public const string FilterStart = "?filter={\"Messages\": {\"$elemMatch\": { \"TimeStamp\": { \"$gt\": {\"$date\":";
        public const string FilterMiddle = "} }, \"TimeStamp\": { \"$lt\": {\"$date\":";
        public const string FilterEnd = "} } } } }&pagesize=1000&page=";
    }

    public class channelsData
    {
        public bool casStave { get; set; }
        public bool fasTasStave { get; set; }
    }

    public class DistributionData
    {
        public string date1UnixTime { get; set; }
        public string date2UnixTime { get; set; }
        public string mode { get; set; }
        public channelsData channels { get; set; }
    }

    class Program
    {
        static HttpClient client = new HttpClient();
        static List<CancellationTokenSource> cancellationTokens = new List<CancellationTokenSource>();

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            int countRes = 0;
            int page = 1;

            #region HttpClientInit
            String encodedAuth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("admin:secret"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", encodedAuth);
            client.BaseAddress = new Uri(Constants.MongoURI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            #endregion

            #region GetInputFromUser
            //Console.WriteLine("Enter Channel Name:");
            //string channel = Console.ReadLine();

            //Console.WriteLine("Enter First Date:");
            //DateTime date1 = DateTime.Parse(Console.ReadLine()).ToUniversalTime();
            //Console.WriteLine("Enter Second Date:");
            //DateTime date2 = DateTime.Parse(Console.ReadLine()).ToUniversalTime();
            //new DateTime(DateTime.Parse(Console.ReadLine()).Ticks, DateTimeKind.Utc)
            #endregion

            string casResultsAsString = "";

            // RabbitMQ init
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var rabbitChannel = connection.CreateModel())
            {
                //Ensure cancellation tokens list is cleared
                cancellationTokens.Clear();


                rabbitChannel.ExchangeDeclare(exchange: "distributionData", type: ExchangeType.Fanout);
                var queueName = rabbitChannel.QueueDeclare().QueueName;
                rabbitChannel.QueueBind(queue: queueName,
                              exchange: "distributionData",
                              routingKey: "");
                Console.WriteLine(" [*] Waiting for logs.");
                var consumer = new EventingBasicConsumer(rabbitChannel);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    DistributionData distributionData = JsonConvert.DeserializeObject<DistributionData>(Encoding.UTF8.GetString(body));
                    //Console.WriteLine(" [x] {0}", channel);
                    Console.WriteLine(distributionData.date1UnixTime + distributionData.date2UnixTime + distributionData.channels);

                    if(distributionData.mode == "start")
                    {
                        long date1 = Convert.ToInt64(distributionData.date1UnixTime);
                        long date2 = Convert.ToInt64(distributionData.date2UnixTime);
                        bool casStave = distributionData.channels.casStave;
                        bool fasTasStave = distributionData.channels.fasTasStave;
                        string casQuery = casStave ? "CasStave" + Constants.FilterStart + date1 + Constants.FilterMiddle
                                        + date2 + Constants.FilterEnd : "";
                        string fasTasQuery = fasTasStave ? "FasTasStave" + Constants.FilterStart + date1 + Constants.FilterMiddle
                                        + date2 + Constants.FilterEnd : "";


                        using (var watch = new SuperWatch())
                        {
                            for (; page <= 2; page++)
                            {
                                try
                                {
                                    HttpResponseMessage casResponse = new HttpResponseMessage();
                                    HttpResponseMessage fasTasResponse = new HttpResponseMessage();
                                    if (casStave)
                                    {
                                        casResponse = await client.GetAsync($"{casQuery}{page.ToString()}");
                                    }
                                    if (fasTasStave)
                                    {
                                        fasTasResponse = await client.GetAsync($"{fasTasQuery}{page.ToString()}");
                                    }
                                    if (casResponse.IsSuccessStatusCode)
                                    {
                                        casResultsAsString = await casResponse.Content.ReadAsStringAsync();
                                    }

                                    JArray casResults = JArray.Parse(casResultsAsString);

                                    //foreach (var res in casResults)
                                    //{

                                    //    DateTime resDate = DateTimeOffset.FromUnixTimeMilliseconds((long)res["CreationDate"]["$date"]).DateTime;
                                    //    Console.WriteLine($"ID: {res["_id"]["$oid"]}, Date: {resDate.ToString()}");
                                    //    ++countRes;
                                    //}


                                    var flag = false;
                                    var ctSource = new CancellationTokenSource();

                                    cancellationTokens.Add(ctSource);

                                    StartSendingMessages("CasStave", ctSource.Token, casResults);
                                    flag = true;


                                    Console.WriteLine($"Number of results: {countRes}");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var ct in cancellationTokens) ct.Cancel();

                        cancellationTokens.Clear();
                    }
                };
                rabbitChannel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.ReadLine();

            }
        }

        static async void StartSendingMessages(string streamType, CancellationToken ct, JArray casResults)
        {
            if (ConfigurationManager.GetSection("StreamSettings/" + streamType) is NameValueCollection config)
            {
                var stream = new StreamWrapper.StreamWrapper(IPAddress.Loopback.ToString(),
                    int.Parse(config["Port"]),
                    double.Parse(config["Delimiter"]),
                    casResults);

                // We don't care if we go back to original context so configure await is false
                await Task.Run(() => stream.SendMessages(ct), ct).ConfigureAwait(false);
            }
        }
    }
}