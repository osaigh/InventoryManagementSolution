using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InventoryServiceLibrary
{
    public class DatabaseService: IDatabaseService
    {
        #region Fields
        private readonly object _lockObject = "lock";
        private IDataStore _DataStore = null;
        #endregion

        #region Properties
        private static readonly DatabaseService current = new DatabaseService();
        /// <summary>
        /// Singleton
        /// </summary>
        public static DatabaseService Current
        {
            get { return current; }
        }
        #endregion

        #region Constructor
        private DatabaseService()
        {
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialization of the database
        /// </summary>
        private void Initialize()
        {
            _DataStore = new DataStore();
        }

        /// <summary>
        /// Sets the data store
        /// </summary>
        /// <param name="dataStore"></param>
        public void SetDataStore(IDataStore dataStore)
        {
            lock (_lockObject)
            {
                this._DataStore = dataStore;
            }
        }

        /// <summary>
        /// Gets the product catalog from the data store
        /// </summary>
        /// <returns></returns>
        public ProductCatalog GetProductCatalog()
        {
            ProductCatalog productCatalog = null;
            lock (_lockObject)
            {
                productCatalog = new ProductCatalog();
                var productCatalogItems = new List<ProductCatalogItem>();
                foreach (var productCatalogItem in _DataStore.GetProductCatalogItems())
                {
                    productCatalogItems.Add(productCatalogItem);
                }

                productCatalog.ProductsCatalogItems = productCatalogItems;

            }
            return productCatalog;
        }

        /// <summary>
        /// Adds the given product with the specified quantity to the order.
        /// also decrements the quantity of the product in stock
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool AddProductQuantityToOrder(string productId, int quantity, string orderId)
        {
            //logic
            bool raiseEvent = false;
            lock (_lockObject)
            {
                raiseEvent = false;
                ProductCatalogItem productCatalogItem = _DataStore.GetProductCatalogItem(productId);
                if (productCatalogItem != null)
                {
                    //only apply this transaction if we have stock
                    if (productCatalogItem.Quantity >= quantity)
                    {
                        raiseEvent = true;
                        productCatalogItem.Quantity -= quantity;

                        //update order
                        AddOrderDetail(productCatalogItem.Product, quantity, orderId);

                        quantity = productCatalogItem.Quantity;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }

            //raise event
            if (raiseEvent)
            {
                OnProductQuantityChangedEvent(new ProductQuantityChangedEventArgs()
                                              {
                                                  ProductId = productId,
                                                  Quantity = quantity,
                                              });
            }

            return true;
        }

        /// <summary>
        /// Removes the given product from the order.
        /// Also adds the given quantity of the product back to the stock
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool RemoveProductFromOrder(string productId, string orderId)
        {
            bool raiseEvent = false;
            int quantity = 0;
            lock (_lockObject)
            {
                raiseEvent = false;
                quantity = 0;
                var order = _DataStore.GetOrder(orderId);
                
                if (order != null )
                {
                    OrderDetail orderDetail = order.OrderDetails.
                                                    FirstOrDefault(_orderDetail => string.Compare(_orderDetail.ProductId, productId, StringComparison.InvariantCultureIgnoreCase) == 0);
                    
                    //Add the product back to the catalog
                    if (orderDetail != null)
                    {
                        var productCatalogItem = _DataStore.GetProductCatalogItem(productId);
                        if (productCatalogItem != null)
                        {
                            productCatalogItem.Quantity += orderDetail.Quantity;
                            quantity = productCatalogItem.Quantity;
                            raiseEvent = true;
                        }

                        //remove the product from the order
                        ((List<OrderDetail>) order.OrderDetails).Remove(orderDetail);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //log warning here
                    return false;
                }
            }

            //raise event
            if (raiseEvent)
            {
                OnProductQuantityChangedEvent(new ProductQuantityChangedEventArgs()
                                              {
                                                  ProductId = productId,
                                                  Quantity = quantity,
                                              });
            }

            return true;
        }

        /// <summary>
        /// creates an order
        /// </summary>
        /// <returns></returns>
        public Order CreateOrder()
        {
            Order order = null;

            lock (_lockObject)
            {
                order = null;
                order = _DataStore.CreateOrder();
            }

            return order;
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool DeleteOrder(string orderId)
        {
            bool raiseEvent = false;
            int quantity = 0;
            List<ProductCatalogItem> itemsUpdated = null;
            lock (_lockObject)
            {
                raiseEvent = false;
                itemsUpdated = new List<ProductCatalogItem>();
                var order = _DataStore.GetOrder(orderId);

                if (order != null)
                {
                    if (order.OrderDetails != null)
                    {
                        foreach (OrderDetail orderDetail in order.OrderDetails)
                        {
                            var productCatalogItem = _DataStore.GetProductCatalogItem(orderDetail.ProductId);
                            if (productCatalogItem != null)
                            {
                                productCatalogItem.Quantity += orderDetail.Quantity;
                                itemsUpdated.Add(productCatalogItem);
                                raiseEvent = true;
                            }

                        }
                    }

                    //delete the order from the store
                    _DataStore.DeleteOrder(orderId);

                }
                else
                {
                    //log warning here
                    return false;
                }
            }

            //raise event
            if (raiseEvent)
            {
                foreach (ProductCatalogItem productCatalogItem in itemsUpdated)
                {
                    OnProductQuantityChangedEvent(new ProductQuantityChangedEventArgs()
                                                  {
                                                      ProductId = productCatalogItem.Product.Id,
                                                      Quantity = productCatalogItem.Quantity,
                                                  });
                }
            }

            return true;
        }

        /// <summary>
        /// Saves an order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool SaveOrder(Order order)
        {
            lock (_lockObject)
            {
                _DataStore.SaveOrder(order);
            }

            return true;
        }
        #endregion

        #region Utility

        /// <summary>
        /// Adds the given product to the order
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <param name="orderId"></param>
        private void AddOrderDetail(Product product, int quantity, string orderId)
        {
            var order = _DataStore.GetOrder(orderId);
            if (order !=  null)
            {
                OrderDetail orderDetail = order.OrderDetails.
                                                FirstOrDefault(_orderDetail => string.Compare(_orderDetail.ProductId, product.Id, StringComparison.InvariantCultureIgnoreCase) == 0);

                if (orderDetail != null)
                {
                    orderDetail.Quantity = quantity;
                }
                else
                {
                    orderDetail = new OrderDetail()
                                  {
                                      ProductId = product.Id,
                                      Quantity = quantity
                                  };
                    ((List<OrderDetail>)order.OrderDetails).Add(orderDetail);
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler<ProductQuantityChangedEventArgs> ProductQuantityChangedEvent;
        protected void OnProductQuantityChangedEvent(ProductQuantityChangedEventArgs args)
        {
            var handler = this.ProductQuantityChangedEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        #endregion
    }
}
