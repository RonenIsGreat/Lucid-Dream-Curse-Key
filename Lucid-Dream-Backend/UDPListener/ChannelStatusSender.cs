using GlobalResourses;
using RabbitMQ.Client;
using System;
using System.Text;

namespace UDPListener
{
    internal class ChannelStatusSender
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        public ChannelStatusSender()
        {
            this.factory = new ConnectionFactory { HostName = "localhost", RequestedHeartbeat = TimeSpan.FromSeconds(60), UserName = "admin", Password = "admin" };
            this.connection = this.factory.CreateConnection();
            this.channel = this.connection.CreateModel();
            channel.ExchangeDeclare("channelStatus", "fanout", false);
        }

        public void SendStatus(string channelStatus)
        {
            // channelStatus is "[channelName] [active/inactive]"
            var body = Encoding.UTF8.GetBytes(channelStatus);
            channel.BasicPublish("channelStatus",
                "",
                null,
                body);
        }
    }
}
