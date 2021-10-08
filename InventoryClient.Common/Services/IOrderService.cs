using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryClient.Common.Models;

namespace InventoryClient.Common.Services
{
    public interface IOrderService
    {
        bool IsReady { get; }
        Order CreateOrder();

        bool DeleteOrder(string orderId);

        bool AddProductQuantityToOrder(string productId, int quantity, string orderId);

        bool RemoveProductFromOrder(string productId, string orderId);

        bool SaveOrder(Order order);
    }
}
