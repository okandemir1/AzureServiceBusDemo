using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using ServiceBusApp.Models;
using ServiceBusApp.Models.Events;
using System.Text;

namespace ServiceBusApp.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsumeSub<OrderCreatedEvent>(ConstInfo.OrderTopic, ConstInfo.OrderCreatedSubname, x => { 
                Console.WriteLine($"OrderCreatedEvent'e mesaj Geldi id:{x.Id}, Name:{x.ProductName}");
            }).Wait();

            ConsumeSub<OrderDeletedEvent>(ConstInfo.OrderTopic, ConstInfo.OrderDeletedSubname, x => {
                Console.WriteLine($"OrderDeletedEvent'e mesaj Geldi id:{x.Id}");
            }).Wait();

            //Uygulama kapanmasın
            Console.ReadLine();
        }

        private static async Task ConsumeSub<T>(string topicName, string subName, Action<T> receivedAction)
        {
            //Queue Yerine Subscription Dinleyeceğiz Şimdi
            //IQueueClient client = new QueueClient(ConstInfo.ConnectionString, queueName);
            ISubscriptionClient client = new SubscriptionClient(ConstInfo.ConnectionString, topicName, subName);
            //ct:CancellationToken

            client.RegisterMessageHandler(async (message, ct) =>
            {
                var model = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));

                receivedAction(model);

                await Task.CompletedTask;

            },
            new MessageHandlerOptions(x => Task.CompletedTask));

            Console.WriteLine($"{typeof(T).Name} dinleniyor....");
        }

        private static async Task ConsumeQueue<T>(string queueName, Action<T> receivedAction)
        {
            IQueueClient client = new QueueClient(ConstInfo.ConnectionString, queueName);
            //ct:CancellationToken

            client.RegisterMessageHandler(async (message, ct) =>
            {
                var model = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));

                receivedAction(model);

                await Task.CompletedTask;

            },
            new MessageHandlerOptions(x => Task.CompletedTask));

            Console.WriteLine($"{typeof(T).Name} dinleniyor....");
        }
    }
}
