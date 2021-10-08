using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    [DataContract]
    public class OrderDetail
    {
        [DataMember]
        public string ProductId { get; set; }
        [DataMember]
        public int Quantity { get; set; }
    }
}
