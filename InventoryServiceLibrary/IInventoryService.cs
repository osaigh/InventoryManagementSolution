using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    public interface IClientCallbackContract
    {
        [OperationContract(IsOneWay = true)]
        void ProductQuantityChanged(string productId, int quantity);
    }

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples", SessionMode = SessionMode.Required, CallbackContract = typeof(IClientCallbackContract))]
    public interface IInventoryService
    {
        // TODO: Add your service operations here
        [OperationContract]
        ProductCatalog GetProductCatalog();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void SubscribeToProductQuantityChanged();

        [OperationContract(IsOneWay = false, IsTerminating = true)]
        void UnsubscribeToProductQuantityChanged();
    }
}
