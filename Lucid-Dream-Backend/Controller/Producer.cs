using System;
using System.Text;
using RabbitMQ.Client;

namespace Controller
{
    internal class Producer
    {
        public void SendMessage(string message, string rKey)
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("channelControl",
                    "direct", true);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("channelControl",
                    rKey,
                    null,
                    body);
                Console.WriteLine(" [x] Sent '{0}':'{1}'", rKey, message);
            }
        }
    }
}