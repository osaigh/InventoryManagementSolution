using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    [DataContract]
    public class ProductCatalogItem
    {
        [DataMember]
        public Product Product { get; set; }
        [DataMember]
        public int Quantity { get; set; }
    }
}
