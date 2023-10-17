using OutSystems.ExternalLibraries.SDK;
using psn.PH.Structures;

namespace psn.PH
{
    [OSInterface(Description = "RabbitMQ client external logic.", Name = "RabbitMQClient_ExternalLogic", IconResourceName = "psn.PH.RabbitMQClientExtIcon.png")]
    public interface IRabbitMQClient_Ext
    {
        [OSAction(Description = "Send a message.", ReturnName = "sendStatus")]
        public bool Send_Ext(Connection connection, ExchangeQueueConfig queue, Message message);
        [OSAction(Description = "Read message(s) from a queue.", ReturnName = "messageResponseResult")]
        public ReadMessagesWrapper Receive_Ext(Connection connection, ExchangeQueueConfig config, int limit);
        [OSAction(Description = "Purge a queue.", ReturnName = "purgeStatus")]
        public bool purgeQueue_Ext(Connection connection, ExchangeQueueConfig config);
        /// <summary>
        /// Retrieve unique build information of this custom library.
        /// </summary>
        [OSAction(Description = "Get unique build information of this custom library.", ReturnName = "buildInfo")]
        public string GetBuildInfo_Ext();
    }
}

