using Xunit;
using Xunit.Abstractions;

using psn.PH.Structures;
namespace psn.PH
{
    public class RabbitMQClient_ExtTests
    {
        private string RABBITMQ_DEFAULT_USER = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "UNKNOWN";
        private string RABBITMQ_DEFAULT_PASS = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "UNKNOWN";
        private string RABBITMQ_DEFAULT_VHOST = Environment.GetEnvironmentVariable("RABBITMQ_VHOST") ?? "UNKNOWN";
        private string RABBITMQ_DEFAULT_HOST = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "UNKNOWN";
        private int RABBITMQ_DEFAULT_PORT = Int32.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672");


        private readonly ITestOutputHelper output;

        public RabbitMQClient_ExtTests(ITestOutputHelper output)
        {
            this.output = output;

        }

        [Fact]
        public void Send_test1()
        {
            var client = new RabbitMQClient_Ext();
            psn.PH.Structures.Connection connection = new psn.PH.Structures.Connection
            {
                userName = RABBITMQ_DEFAULT_USER,
                password = RABBITMQ_DEFAULT_PASS,
                hostName = RABBITMQ_DEFAULT_HOST,
                virtualHost = RABBITMQ_DEFAULT_VHOST,
                port = RABBITMQ_DEFAULT_PORT,
            };
            psn.PH.Structures.ExchangeQueueConfig config = new psn.PH.Structures.ExchangeQueueConfig
            {
                exchangeName = "exchange1",
                queueName = "queue1",
                type = "classic",
                isDurable = true,
                isInternal = false,
                isAutoDelete = false,
            };
            Message msg = new psn.PH.Structures.Message
            {
                data = "Hello World from Send_test1!",
            };
            Assert.True(client.Send_Ext(connection, config, msg));
            client.purgeQueue_Ext(connection, config);
        }

        [Fact]
        public void Receive_test1()
        {
            var client = new RabbitMQClient_Ext();
            psn.PH.Structures.Connection connection = new psn.PH.Structures.Connection
            {
                userName = RABBITMQ_DEFAULT_USER,
                password = RABBITMQ_DEFAULT_PASS,
                hostName = RABBITMQ_DEFAULT_HOST,
                virtualHost = RABBITMQ_DEFAULT_VHOST,
                port = RABBITMQ_DEFAULT_PORT,
            };
            psn.PH.Structures.ExchangeQueueConfig config = new psn.PH.Structures.ExchangeQueueConfig
            {
                queueName = "queue1", // used for sending and receiving msg
                isExclusive = false, // used for receiving msg
                isDurable = true, // used for sending and receiving msg
                exchangeName = "exchange1", // used for sending msg 
                type = "classic", // used for sending msg
                isInternal = false, // used for sending msg
            };
            for (int i = 0; i < 3; i++)
            {
                Message msg = new psn.PH.Structures.Message
                {
                    data = "Hello World from Receive_test1! " + i,
                };
                client.Send_Ext(connection, config, msg);
            }

            Thread.Sleep(10000); // wait for a while before consuming messages

            ReadMessagesWrapper reply = client.Receive_Ext(connection, config, 10);
            output.WriteLine("Message count:" + reply.messages.Count);
            Assert.True(reply.messages.Count > 0);
            if (reply.messages.Count > 0)
            {
                Message[] msgs = reply.messages.ToArray();
                foreach (Message msg in msgs)
                {
                    output.WriteLine("Content:" + msg.data);
                }
            }
        }

        [Fact]
        public void GetBuildInfo_Ext_test1()
        {
            var client = new RabbitMQClient_Ext();
            string buildInfo = client.GetBuildInfo_Ext();
            output.WriteLine(buildInfo);
            Assert.True(buildInfo.Length > 0);
        }
    }
}

