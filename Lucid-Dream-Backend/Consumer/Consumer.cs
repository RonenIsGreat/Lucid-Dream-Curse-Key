using System;
using System.Text;
using GlobalResourses;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UDPListener;

namespace Consumer
{
    public class Consumer
    {
        private readonly SaveStream.SaveStreamHelper _streamSaver;
        private readonly UdpListener _udpClient;

        public Consumer(UdpListener uDp, string dbConnectionUrl)
        {
            _udpClient = uDp;
            _streamSaver = new SaveStream.SaveStreamHelper( dbConnectionUrl, 30);
        }

        public void ListenToQueue()
        {
            ConnectToRabbitMQ(out IModel channel, out string queueName);

            EventingBasicConsumer consumer = SetupRabbitMQConsumer(channel);

            // Start the consumer
            channel.BasicConsume(queueName, true, consumer);
        }

        private EventingBasicConsumer SetupRabbitMQConsumer(IModel channel)
        {
            // Define what happens when data is received from the RabbitMQ exchange
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                if (message.Equals("ON"))
                {
                    _udpClient.DataReceivedDelegate += UdpClientDataReceivedDelegate;
                    _udpClient.StartListener();
                }
                else if (message.Equals("OFF"))
                {
                    _udpClient.StopListener();
                    _udpClient.DataReceivedDelegate -= UdpClientDataReceivedDelegate;
                }

                Console.WriteLine(" [x] Received '{0}':'{1}'",
                    routingKey, message);
            };
            return consumer;
        }

        private void ConnectToRabbitMQ(out IModel channel, out string queueName)
        {
            // Set up a RabbitMQ Connection for a specific port
            // and bind to a RabbitMQ exchange
            ChannelDetails port = _udpClient.Param;
            ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };
            IConnection connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare("channelControl", "direct", true);
            queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueName,
                "channelControl",
                port.GetName().ToString());

            Console.WriteLine(" [*] {0} Is waiting for messages.", port.GetName().ToString());
        }

        private void UdpClientDataReceivedDelegate(object sender, StateObject data)
        {
            UdpListener currentListener = (UdpListener) sender;

            var succeeded = _streamSaver.SaveData(data.buffer);
            if (!succeeded) Console.WriteLine("Failed to save message");
        }
    }
}