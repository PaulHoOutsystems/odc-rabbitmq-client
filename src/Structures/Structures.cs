using OutSystems.ExternalLibraries.SDK;

namespace psn.PH.Structures
{
    [OSStructure(Description = "Connection details")]
    public struct Connection
    {
        [OSStructureField(DataType = OSDataType.Text, Description = "Host name", IsMandatory = true)]
        public string hostName;
        [OSStructureField(DataType = OSDataType.Text, Description = "Virtual host", IsMandatory = true)]
        public string virtualHost;
        [OSStructureField(DataType = OSDataType.Integer, Description = "Port", IsMandatory = true)]
        public int port;
        [OSStructureField(DataType = OSDataType.Text, Description = "User name", IsMandatory = true)]
        public string userName;
        [OSStructureField(DataType = OSDataType.Text, Description = "Password", IsMandatory = true)]
        public string password;
    }

    [OSStructure(Description = "Message")]
    public struct Message
    {
        [OSStructureField(DataType = OSDataType.Text, Description = "Data", IsMandatory = true)]
        public string data;
    }

    [OSStructure(Description = "A record of messages and indicator whether there are more messages")]
    public struct ReadMessagesWrapper
    {
        public List<Message> messages;
        public bool hasMore;
    }

    [OSStructure(Description = "Queue")]
    public struct ExchangeQueueConfig
    {
        [OSStructureField(DataType = OSDataType.Text, Description = "Exchange name", IsMandatory = true)]
        public string exchangeName;
        [OSStructureField(DataType = OSDataType.Text, Description = "Queue name", IsMandatory = true)]
        public string queueName;
        [OSStructureField(DataType = OSDataType.Text, Description = "Queue / Exchange type. For Queue, can be: classic, quorum, stream. For Exchange, can be: direct, fanout, topic, headers", IsMandatory = true)]
        public string type;
        [OSStructureField(DataType = OSDataType.Boolean, Description = "Is durable?", IsMandatory = true)]
        public bool isDurable;
        [OSStructureField(DataType = OSDataType.Boolean, Description = "Is internal? Note: used for exchange only. If yes, clients cannot publish to this exchange directly. It can only be used with exchange to exchange bindings.", IsMandatory = false)]
        public bool isInternal;
        [OSStructureField(DataType = OSDataType.Boolean, Description = "Is exclusive? Note: used for queue only. If yes, such a queue will be deleted when its declaring connection closes.", IsMandatory = false)]
        public bool isExclusive;
        [OSStructureField(DataType = OSDataType.Boolean, Description = "Is autodelete? Note: used for exchange only", IsMandatory = false)]
        public bool isAutoDelete;
    }

}