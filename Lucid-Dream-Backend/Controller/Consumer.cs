using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Controller
{
    public class Consumer
    {
        private readonly UDPListener udpClient;

        public Consumer(UDPListener uDP)
        {
            udpClient = uDP;
        }

        public void ListenToQueue()
        {
            var port = udpClient._Param;
            var factory = new ConnectionFactory {HostName = "localhost"};
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("channelControl",
                "direct", true);
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queueName,
                "channelControl",
                port.GetName().ToString());


            Console.WriteLine(" [*] {0} Is waiting for messages.", port.GetName().ToString());

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                if (message.Equals("ON"))
                    udpClient.StartListener();
                else if (message.Equals("OFF"))
                    udpClient.StopListener();
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                    routingKey, message);
            };
            channel.BasicConsume(queueName,
                true,
                consumer);
        }
    }
}