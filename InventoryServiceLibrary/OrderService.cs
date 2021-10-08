using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class OrderService : IOrderService
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public OrderService()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a given product to the order with the quantity
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool AddProductQuantityToOrder(string productId, int quantity, string orderId)
        {
            SimulateNetworkDelay();
            Debug.WriteLine("Received message:" + productId);
            return DatabaseService.Current.AddProductQuantityToOrder(productId,quantity,orderId);
        }

        /// <summary>
        /// Creates an Order
        /// </summary>
        /// <returns></returns>
        public Order CreateOrder()
        {
            SimulateNetworkDelay();
            return DatabaseService.Current.CreateOrder();
        }

        /// <summary>
        /// Deletes an Order with the given orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool DeleteOrder(string orderId)
        {
            SimulateNetworkDelay();
            return DatabaseService.Current.DeleteOrder(orderId);
        }

        /// <summary>
        /// Removes a product from an order
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool RemoveProductFromOrder(string productId, string orderId)
        {
            SimulateNetworkDelay();
            return DatabaseService.Current.RemoveProductFromOrder(productId,orderId);
        }

        /// <summary>
        /// Saves an order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool SaveOrder(Order order)
        {
            SimulateNetworkDelay();
            return DatabaseService.Current.SaveOrder(order); ;
        }

        /// <summary>
        /// Simulates network delay
        /// </summary>
        private void SimulateNetworkDelay()
        {
            Thread.Sleep(1500);
        }
        #endregion
    }
}
