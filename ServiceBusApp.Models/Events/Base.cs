using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusApp.Models.Events
{
    public class Base
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
