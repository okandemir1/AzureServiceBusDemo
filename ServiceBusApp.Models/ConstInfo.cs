namespace ServiceBusApp.Models
{
    public class ConstInfo
    {
        public const string ConnectionString = "Primary Connection String";

        public const string OrderCreatedQueueName = "OrderCreatedQueue";
        public const string OrderDeletedQueueName = "OrderDeletedQueue";

        public const string OrderTopic = "OrderTopic";
        public const string OrderCreatedSubname = "OrderCreatedSub";
        public const string OrderDeletedSubname = "OrderDeletedSub";
    }
}
