using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceBusApp.Models;
using ServiceBusApp.Models.Dto;
using ServiceBusApp.Models.Events;
using ServiceBusApp.ProducesApi.Services;

namespace ServiceBusApp.ProducesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AzureService azureService;

        public OrderController(AzureService azureService)
        {
            this.azureService = azureService;
        }
        //Sipariş oluşturma ve silme tamam şimdi consumer app geliştirmem gerek

        [HttpPost]
        public async Task CreateOrder(OrderDto order)
        {
            //siparişi database gönderebiliriz

            var orderCreatedEvent = new OrderCreatedEvent()
            {
                CreateDate = DateTime.Now,
                Id = order.Id,
                ProductName = order.ProductName,
            };

            //Not:Yoksa oluşturacak bunu azureservice tarafında yapmak mantıklı olan şimdilik buraya ekledim
            //await azureService.CreateQueueIfNotExist(ConstInfo.OrderCreatedQueueName);
            //await azureService.SendMessageToQueue(ConstInfo.OrderCreatedQueueName, orderCreatedEvent);

            //Topic Queue gibi çalışmıyor subscriptionları mevcut var mı diye onları da kontrol etmek gerek
            await azureService.CreateTopicIfNotExist(ConstInfo.OrderTopic);
            
            //Subscriptionları oluşturalım
            await azureService.CreateSubscriptionIfNotExists(ConstInfo.OrderTopic, ConstInfo.OrderCreatedSubname, "OrderCreated", "OrderCreatedOnly");

            await azureService.SendMessageToTopic(ConstInfo.OrderTopic, orderCreatedEvent, "OrderCreated");
        }

        [HttpDelete("{id}")]
        public async Task DeleteOrder(int id)
        {
            var orderDeletedEvent = new OrderDeletedEvent()
            {
                CreateDate = DateTime.Now,
                Id = id,
            };

            //await azureService.CreateQueueIfNotExist(ConstInfo.OrderDeletedQueueName);
            //await azureService.SendMessageToQueue(ConstInfo.OrderDeletedQueueName, orderDeletedEvent);

            await azureService.CreateTopicIfNotExist(ConstInfo.OrderTopic);

            //Subscriptionları oluşturalım
            await azureService.CreateSubscriptionIfNotExists(ConstInfo.OrderTopic, ConstInfo.OrderDeletedSubname, "OrderDeleted", "OrderDeletedOnly");
            //Her hangi bir kural olmadığı için hem created hemde deleted'e geldi
            //Azure panel'den sub'a filter ekledim CorrelationFilter yöntemi ile custom prop olarak ekledim
            await azureService.SendMessageToTopic(ConstInfo.OrderTopic, orderDeletedEvent, "OrderDeleted");
        }
    }
}
