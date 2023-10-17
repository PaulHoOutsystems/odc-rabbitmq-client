using System.Reflection;
using RabbitMQ.Client;
using psn.PH.Structures;

namespace psn.PH
{
    public class RabbitMQClient_Ext : IRabbitMQClient_Ext
    {
        private IConnection getConnection(Connection connection)
        {
            string hostName = connection.hostName.Equals("secure-gateway") ? Environment.GetEnvironmentVariable("SECURE_GATEWAY") ?? "hostname-undefined" : connection.hostName;
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = connection.userName,
                Password = connection.password,
                VirtualHost = connection.virtualHost,
                HostName = hostName,
                Port = connection.port,
                AutomaticRecoveryEnabled = true,
            };

            IConnection conn = factory.CreateConnection();
            return conn;
        }
        public bool Send_Ext(Connection connection, ExchangeQueueConfig config, Message message)
        {
            IConnection conn = getConnection(connection);
            IModel channel = conn.CreateModel();
            channel.ExchangeDeclare(config.exchangeName, ExchangeType.Direct, config.isDurable, config.isAutoDelete);
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(message.data);
            IBasicProperties props = channel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;

            channel.BasicPublish(config.exchangeName, config.queueName, true, props, messageBodyBytes);
            channel.Close();
            conn.Close();
            return true;
        }

        public ReadMessagesWrapper Receive_Ext(Connection connection, ExchangeQueueConfig config, int limit)
        {
            IConnection conn = getConnection(connection);
            IModel channel = conn.CreateModel();
            var response = channel.QueueDeclare(config.queueName, config.isDurable, config.isInternal, config.isAutoDelete, null);
            uint maxMessagesCount = response.MessageCount;
            bool autoAck = false;
            List<Message> qmessages = new List<Message>();
            if (maxMessagesCount > limit)
            {
                int currentCount = 0;
                BasicGetResult qresult = channel.BasicGet(config.queueName, autoAck);
                while (qresult != null && currentCount < limit)
                {
                    var body = qresult.Body.Span;
                    Message currentMsg = new Message { data = System.Text.Encoding.UTF8.GetString(body) };
                    qmessages.Add(currentMsg);
                    channel.BasicAck(qresult.DeliveryTag, false);
                    currentCount++;
                    qresult = channel.BasicGet(config.queueName, autoAck);
                }
            }
            else
            {
                BasicGetResult qresult = channel.BasicGet(config.queueName, autoAck);
                while (qresult != null)
                {
                    var body = qresult.Body.Span;
                    Message currentMsg = new Message { data = System.Text.Encoding.UTF8.GetString(body) };
                    qmessages.Add(currentMsg);
                    channel.BasicAck(qresult.DeliveryTag, false);
                    qresult = channel.BasicGet(config.queueName, autoAck);
                }
            }
            ReadMessagesWrapper result = new ReadMessagesWrapper { messages = qmessages, hasMore = (maxMessagesCount > limit) }; ;
            channel.Close();
            conn.Close();
            return result;
        }

        public bool purgeQueue_Ext(Connection connection, ExchangeQueueConfig config)
        {
            IConnection conn = getConnection(connection);
            IModel channel = conn.CreateModel();
            channel.QueuePurge(config.queueName);
            return true;
        }


        private string ReadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;
            if (assembly.GetManifestResourceStream(resourcePath) != null)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourcePath)!)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return string.Empty;
        }

        public string GetBuildInfo_Ext()
        {
            return ReadResource("psn.PH.buildinfo.txt");
        }
    }
}
