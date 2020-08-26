using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace LD_18
{
    class Producer
    {
        public void SendMessage(string message, string rKey)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "channelControl",
                                        type: "direct", durable: true);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "channelControl",
                                     routingKey: rKey,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent '{0}':'{1}'", rKey, message);
            }
        }
    }
}
