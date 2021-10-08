using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class InventoryService : IInventoryService, IDisposable
    {
        #region Fields

        private IClientCallbackContract clientCallback;
        #endregion

        #region Constructor

        public InventoryService()
        {
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the service
        /// </summary>
        private void Initialize()
        {
            DatabaseService.Current.ProductQuantityChangedEvent += DatabaseServiceProductQuantityChangedEventHandler;
        }

        /// <summary>
        /// Gets the product catalog from the store
        /// </summary>
        /// <returns></returns>
        public ProductCatalog GetProductCatalog()
        {
            SimulateNetworkDelay();
            return DatabaseService.Current.GetProductCatalog();
        }

        /// <summary>
        /// Subscribe to the ProductQuantityChanged event
        /// </summary>
        public void SubscribeToProductQuantityChanged()
        {
            SimulateNetworkDelay();
            clientCallback = OperationContext.Current.GetCallbackChannel<IClientCallbackContract>();

        }

        /// <summary>
        /// Unsubscribe to the ProductQuantityChanged event
        /// </summary>
        public void UnsubscribeToProductQuantityChanged()
        {
            SimulateNetworkDelay();
            clientCallback = null;
        }

        /// <summary>
        /// Simulates network delay
        /// </summary>
        private void SimulateNetworkDelay()
        {
            Thread.Sleep(1500);
        }
        #endregion

        #region Events
        /// <summary>
        /// A handler to the DatabaseService ProductQuantityChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatabaseServiceProductQuantityChangedEventHandler(object sender, ProductQuantityChangedEventArgs e)
        {
            Task.Run(() =>
                     {
                         clientCallback.ProductQuantityChanged(e.ProductId,e.Quantity);
                     });

        }
        #endregion

        #region IDispose
        public void Dispose()
        {
            DatabaseService.Current.ProductQuantityChangedEvent -= DatabaseServiceProductQuantityChangedEventHandler;
        }
        #endregion
    }
}
