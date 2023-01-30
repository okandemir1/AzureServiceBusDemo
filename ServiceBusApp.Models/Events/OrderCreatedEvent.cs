using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusApp.Models.Events
{
    public class OrderCreatedEvent : Base
    {
        public string ProductName { get; set; }
    }
}
