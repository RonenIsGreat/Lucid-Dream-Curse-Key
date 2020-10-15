using RabbitMQ.Client;
using System.Text;

namespace UDPListener
{
    internal class ChannelStatusSender
    {
        public void SendStatus(string channelStatus)
        {
            // channelStatus is "[channelName] [active/inactive]"
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("channelStatus",
                    "fanout", false);
                var body = Encoding.UTF8.GetBytes(channelStatus);
                channel.BasicPublish("channelStatus",
                    "",
                    null,
                    body);
            }
        }
    }
}
