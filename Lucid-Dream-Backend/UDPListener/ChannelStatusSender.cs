using GlobalResourses;
using RabbitMQ.Client;
using System;
using System.Text;

namespace UDPListener
{
    internal class ChannelStatusSender
    {
        public void SendStatusInactive(string channelName)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("channelStatus",
                    "direct", true);
                var body = Encoding.UTF8.GetBytes(channelName);
                channel.BasicPublish("channelStatus",
                    "",
                    null,
                    body);
            }
        }
    }
}
