using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UDPListener;

namespace Controller
{
    public class Consumer
    {
        private readonly SaveStreamHelper.SaveStreamHelper _streamSaver;
        private readonly UdpListener _udpClient;

        public Consumer(UdpListener uDP, string savePath)
        {
            _udpClient = uDP;
            _streamSaver = new SaveStreamHelper.SaveStreamHelper(savePath);
        }

        public void ListenToQueue()
        {
            var port = _udpClient.Param;
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

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                if (message.Equals("ON"))
                {
                    _udpClient.OnDataReceived += _udpClient_OnDataReceived;
                    _udpClient.StartListener();
                }
                else if (message.Equals("OFF"))
                {
                    _udpClient.StopListener();
                    _udpClient.OnDataReceived -= _udpClient_OnDataReceived;
                }

                Console.WriteLine(" [x] Received '{0}':'{1}'",
                    routingKey, message);
            };
            channel.BasicConsume(queueName,
                true,
                consumer);
        }

        private void _udpClient_OnDataReceived(object sender, StateObject data)
        {
            UdpListener currentListener = (UdpListener) sender;

            //This date format can be saved as file name
            var dateAsString = DateTime.Now.ToString("yyyy-dd-M--HH-mm");
            var succeeded = _streamSaver.SaveData(data.buffer, dateAsString);
            if (!succeeded) Console.WriteLine("Failed to save message");
        }
    }
}