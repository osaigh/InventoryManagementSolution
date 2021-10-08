using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples",SessionMode = SessionMode.Required)]
    public interface IOrderService
    {
        [OperationContract]
        Order CreateOrder();

        [OperationContract]
        bool DeleteOrder(string orderId);

        [OperationContract]
        bool AddProductQuantityToOrder(string productId, int quantity, string orderId);

        [OperationContract]
        bool RemoveProductFromOrder(string productId, string orderId);

        [OperationContract]
        bool SaveOrder(Order order);
    }
}
