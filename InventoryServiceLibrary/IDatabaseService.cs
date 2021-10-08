using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    public interface IDatabaseService
    {
        event EventHandler<ProductQuantityChangedEventArgs> ProductQuantityChangedEvent;
        ProductCatalog GetProductCatalog();
        bool AddProductQuantityToOrder(string productId, int quantity, string orderId);
        bool RemoveProductFromOrder(string productId, string orderId);
        Order CreateOrder();
        bool DeleteOrder(string orderId);
        bool SaveOrder(Order order);
        void SetDataStore(IDataStore dataStore);
    }
}
