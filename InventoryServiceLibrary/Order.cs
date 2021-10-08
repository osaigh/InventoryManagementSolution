using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string CustomerId { get; set; }

        [DataMember]
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
