using GlobalResourses;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;
using System.Threading;

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
        }

        public void SendStatus(string channelStatus)
        {
            try
            {
                this.connection = this.factory.CreateConnection();
                this.channel = this.connection.CreateModel();
                channel.ExchangeDeclare("channelStatus", "fanout", false);
                // channelStatus is "[channelName] [active/inactive]"
                var body = Encoding.UTF8.GetBytes(channelStatus);
                channel.BasicPublish("channelStatus",
                    "",
                    null,
                    body);
            }
            catch (Exception e) { 
                if(e is BrokerUnreachableException)
                {
                    Thread.Sleep(1000);
                    this.connection = this.factory.CreateConnection();
                    this.channel = this.connection.CreateModel();
                    channel.ExchangeDeclare("channelStatus", "fanout", false);
                    // channelStatus is "[channelName] [active/inactive]"
                    var body = Encoding.UTF8.GetBytes(channelStatus);
                    channel.BasicPublish("channelStatus",
                        "",
                        null,
                        body);
                }
            }
        }
    }
}
