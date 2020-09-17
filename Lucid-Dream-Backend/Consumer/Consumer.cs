using System;
using System.Text;
using GlobalResourses;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SaveStream;
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
            ChannelDetails port = _udpClient.Param;
            ConnectionFactory factory = new ConnectionFactory {HostName = "localhost"};
            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.ExchangeDeclare("channelControl",
                "direct", true);
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queueName,
                "channelControl",
                port.GetName().ToString());


            Console.WriteLine(" [*] {0} Is waiting for messages.", port.GetName().ToString());

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
            channel.BasicConsume(queueName,
                true,
                consumer);
        }

        private void UdpClientDataReceivedDelegate(object sender, StateObject data)
        {
            UdpListener currentListener = (UdpListener) sender;

            //This date format can be saved as file name
            var dateAsString = DateTime.Now.ToString("yyyy-dd-M--HH-mm");
            var succeeded = _streamSaver.SaveData(data.buffer, dateAsString);
            if (!succeeded) Console.WriteLine("Failed to save message");
        }
    }
}