using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    public interface IDataStore
    {
        IEnumerable<ProductCatalogItem> GetProductCatalogItems();
        ProductCatalogItem GetProductCatalogItem(string productId);
        Order CreateOrder();
        Order GetOrder(string orderId);
        void DeleteOrder(string orderId);
        void SaveOrder(Order order);
    }
}
