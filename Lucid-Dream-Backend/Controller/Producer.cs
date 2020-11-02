using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

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
        }

        public void SendMessage(string message, string rKey)
        {
            IConnection connection;
            try
            {
                connection = this.factory.CreateConnection();
            }
            catch (BrokerUnreachableException e)
            {
                Thread.Sleep(1000);
                connection = this.factory.CreateConnection();
            }
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("channelControl", "direct", true);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("channelControl",
                rKey,
                null,
                body);
            //Console.WriteLine(" [x] Sent '{0}':'{1}'", rKey, message);
        }
    }
}