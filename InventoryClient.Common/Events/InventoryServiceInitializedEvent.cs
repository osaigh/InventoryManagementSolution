using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace InventoryClient.Common.Events
{
    public class InventoryServiceInitializedEvent: PubSubEvent<string>
    {
    }
}
