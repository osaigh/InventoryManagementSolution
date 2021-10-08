using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryClient.Common.Models;
using Prism.Events;

namespace InventoryClient.Common.Events
{
    public class OrderCreatedEvent:PubSubEvent<Order>
    {
    }
}
