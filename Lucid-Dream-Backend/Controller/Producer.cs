using System;
using System.Text;
using RabbitMQ.Client;

namespace Controller
{
    internal class Producer
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        public Producer()
        {
            this.factory = new ConnectionFactory { HostName = "localhost", RequestedHeartbeat = TimeSpan.FromSeconds(60), UserName="admin", Password="admin" };
            this.connection = this.factory.CreateConnection();
            this.channel = this.connection.CreateModel();
            channel.ExchangeDeclare("channelControl", "direct", true);
        }

        public void SendMessage(string message, string rKey)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("channelControl",
                rKey,
                null,
                body);
            Console.WriteLine(" [x] Sent '{0}':'{1}'", rKey, message);
        }
    }
}