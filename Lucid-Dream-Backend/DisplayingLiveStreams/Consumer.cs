using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;



namespace DisplayingLiveStreams
{
    class Consumer
    {
        public void ListenToQueue(string rKey)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "channelControl",
                                    type: "direct", durable: true);
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                                exchange: "channelControl",
                                routingKey: rKey);


            Console.WriteLine(" [*] {0} Is waiting for messages.", rKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey, message);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

        }
    }
}
