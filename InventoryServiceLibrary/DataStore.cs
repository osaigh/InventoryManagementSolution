using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    public class DataStore :IDataStore
    {
        #region Fields
        private Dictionary<string, ProductCatalogItem> _ProductCatalogItems = new Dictionary<string, ProductCatalogItem>();
        private Dictionary<string, Order> _Orders = new Dictionary<string, Order>();
        private int orderCount = 1;
        #endregion

        #region Properties

        #endregion

        #region Constructor

        public DataStore()
        {
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the store
        /// </summary>
        protected void Initialize()
        {
            foreach (ProductCatalogItem productCatalogItem in TestData.GetTestProductCatalogItemData())
            {
                _ProductCatalogItems.Add(productCatalogItem.Product.Id, productCatalogItem);
            }

            var order = TestData.GetTestOrder();
            this._Orders.Add(order.Id, order);
        }

        /// <summary>
        /// Adds a ProductCatalogItem to the store
        /// </summary>
        /// <param name="productCatalogItem"></param>
        public void AddProductCatalogItem(ProductCatalogItem productCatalogItem)
        {
            _ProductCatalogItems.Add(productCatalogItem.Product.Id, productCatalogItem);
        }

        /// <summary>
        /// Adds an order
        /// </summary>
        /// <param name="order"></param>
        public void AddOrder(Order order)
        {
            _Orders.Add(order.Id, order);
        }

        /// <summary>
        /// Gets all ProductCatalogItems
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductCatalogItem> GetProductCatalogItems()
        {
            return this._ProductCatalogItems.Values.ToList();
        }

        /// <summary>
        /// Gets a ProductCatalogItem given the productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ProductCatalogItem GetProductCatalogItem(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return null;
            }
            return this._ProductCatalogItems.ContainsKey(productId) ? _ProductCatalogItems[productId] : null;
        }

        /// <summary>
        /// Gets an order given an orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Order GetOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return null;
            }
            return this._Orders.ContainsKey(orderId) ? _Orders[orderId] : null;
        }

        /// <summary>
        /// creates an order
        /// </summary>
        /// <returns></returns>
        public Order CreateOrder()
        {
            var order = new Order()
                        {
                            Id = "Order "+orderCount.ToString(),
                            OrderDetails = new List<OrderDetail>(),
                        };
            orderCount++;
            _Orders.Add(order.Id, order);

            return order;
        }

        /// <summary>
        /// Deletes an order and returns all items back to stock
        /// </summary>
        /// <param name="orderId"></param>
        public void DeleteOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return ;
            }

            if (_Orders.ContainsKey(orderId))
            {
                var order = _Orders[orderId];
                order.Id = null;
                _Orders.Remove(orderId);
            }
        }

        /// <summary>
        /// Persist the order
        /// </summary>
        /// <param name="order"></param>
        public void SaveOrder(Order order)
        {
            //Simulate save to database here
        }
        #endregion
    }
}
