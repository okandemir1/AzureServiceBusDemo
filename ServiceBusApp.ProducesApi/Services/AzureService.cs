using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using ServiceBusApp.Models;
using System.Text;

namespace ServiceBusApp.ProducesApi.Services
{
    public class AzureService
    {
        private readonly ManagementClient managementClient;
        public AzureService(ManagementClient managementClient)
        {
            this.managementClient = managementClient;
        }

        //Queue'ya mesajı gönderdik
        public async Task SendMessageToQueue(string queueName, object messageContent, string messageType = "")
        {
            IQueueClient client = new QueueClient(ConstInfo.ConnectionString, queueName);

            await SendMessage(client, messageContent, messageType);
        }

        public async Task CreateQueueIfNotExist(string queueName)
        {
            if (!await managementClient.QueueExistsAsync(queueName))
                await managementClient.CreateQueueAsync(queueName);
        }

        public async Task SendMessageToTopic(string topicName, object messageContent, string messageType="")
        {
            ITopicClient client = new TopicClient(ConstInfo.ConnectionString, topicName);
            
            await SendMessage(client, messageContent, messageType);
        }

        public async Task CreateSubscriptionIfNotExists(string topicName, string subscriptionName, string messageType = "", string ruleName = "")
        {
            if (await managementClient.SubscriptionExistsAsync(topicName, subscriptionName))
                return;

            if(!string.IsNullOrEmpty(messageType))
            {
                //CorrelationFilter'i panel üzerinden yaptım ama bunu kod ile nasıl yaparız onun çözümü burada
                SubscriptionDescription sd = new SubscriptionDescription(topicName, subscriptionName);

                CorrelationFilter filter = new();
                //Önceden tanımlı olanlar zaten var custom yapmak istiyorsan properties kısmını kullabilirsin
                filter.Properties["MessageType"] = messageType;

                RuleDescription rd = new(ruleName ?? messageType+"Rule",filter);

                await managementClient.CreateSubscriptionAsync(sd,rd);
            }
            else
                await managementClient.CreateSubscriptionAsync(topicName, subscriptionName);
        }

        public async Task CreateTopicIfNotExist(string topicName)
        {
            if (!await managementClient.TopicExistsAsync(topicName))
                await managementClient.CreateTopicAsync(topicName);
        }

        //Kod tekrarı olmasın
        private async Task SendMessage(ISenderClient client, object messageContent, string messageType = "")
        {
            var byteArr = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageContent));

            var message = new Message(byteArr);
            message.UserProperties["MessageType"] = messageType;

            await client.SendAsync(message);
        }
    }
}
