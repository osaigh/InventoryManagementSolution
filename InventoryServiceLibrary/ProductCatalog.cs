using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    [DataContract]
    public class ProductCatalog
    {
        [DataMember]
        public IEnumerable<ProductCatalogItem> ProductsCatalogItems { get; set; }
    }
}
