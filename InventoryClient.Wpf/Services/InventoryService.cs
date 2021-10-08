using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using InventoryClient.Common.Events;
using InventoryClient.Common.Models;
using InventoryClient.Wpf.InventoryServiceRef;
using Polly;
using Polly.Retry;
using Prism.Events;
using IInventoryService = InventoryClient.Common.Services.IInventoryService;
using ProductCatalogItem = InventoryClient.Common.Models.ProductCatalogItem;

namespace InventoryClient.Wpf.Services
{
    public class InventoryService : IInventoryService, IInventoryServiceCallback, INotifyPropertyChanged, IDisposable
    {
        #region Fields

        private static int RETRY_ATTEMPTS = 10;
        private InventoryServiceClient _InventoryServiceClient = null;
        private IEventAggregator _EventAggregator;
        private Dictionary<string, ProductCatalogItem> _ProductCatalogItems=null;
        #endregion

        #region Properties

        private bool isServiceInitialized = false;
        public bool IsServiceInitialized
        {
            get { return isServiceInitialized;}
            private set { isServiceInitialized = value;OnPropertyChanged("IsServiceInitialized"); }
        }

        private ObservableCollection<ProductCatalogItem> productCatalogItems = new ObservableCollection<ProductCatalogItem>();

        public ObservableCollection<ProductCatalogItem> ProductCatalogItems
        {
            get { return productCatalogItems; }
            private set
            {
                productCatalogItems = value;
                OnPropertyChanged("ProductCatalogItems");
            }
        }
        #endregion 

        #region Constructor
        public InventoryService(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;
            Initialize();
        }
        #endregion

        #region Methods
        protected void Initialize()
        {
            Debug.WriteLine($"InventoryService initializing");
            _ProductCatalogItems = new Dictionary<string, ProductCatalogItem>();
            _ServiceInitializedDelegate = new ServiceInitializedDelegate(OnServiceInitialized);
            StartClient();
        }

        /// <summary>
        /// starts the InventoryServiceClient
        /// </summary>
        protected void StartClient()
        {
            _InventoryServiceClient = new InventoryServiceClient(new InstanceContext(this));

            //Use a retry policy to handle network delays and server not running yet.
            Task.Run(() =>
                     {
                         var policy = RetryPolicy.Handle<SocketException>()
                                                 .Or<System.ServiceModel.EndpointNotFoundException>()
                                                 .Or<Exception>()
                                                 .WaitAndRetry(RETRY_ATTEMPTS, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                                               (ex, time) =>
                                                               {
                                                                   //Log error
                                                                   Debug.WriteLine($"The following error occured while trying to start InventoryServiceClient {ex.Message}");
                                                               });

                         policy.Execute(() =>
                                        {
                                            Thread.Sleep(2000);
                                            _InventoryServiceClient.SubscribeToProductQuantityChanged();
                                            if (_InventoryServiceClient.State != CommunicationState.Opened)
                                            {
                                                throw new Exception($"InventoryServiceClient is in the {_InventoryServiceClient.State}");
                                            }
                                        });

                         if (_InventoryServiceClient.State == CommunicationState.Opened)
                         {
                             System.Windows.Application.Current.Dispatcher.Invoke(_ServiceInitializedDelegate);
                         }
                         else
                         {
                             MessageBox.Show("InventoryServiceClient unable to connect to server in a timely manner. App will shutdown");
                             System.Windows.Application.Current.Shutdown();
                         }
                     });
        }
        #endregion

        #region IInventoryService
        /// <summary>
        /// Gets the product catalog
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductCatalogItem> GetProductCatalog()
        {
            return _ProductCatalogItems.Values.ToList();
        }
        #endregion

        #region IInventoryServiceCallback
        /// <summary>
        /// A callback from the server when a product quantity is updated
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        public void ProductQuantityChanged(string productId, int quantity)
        {
            //Update the product catalog item
            if (_ProductCatalogItems.ContainsKey(productId))
            {
                _ProductCatalogItems[productId].Quantity = quantity;

                //Raise ProductQuantityChangedEvent
                _EventAggregator.GetEvent<ProductQuantityChangedEvent>().Publish(productId);
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            _EventAggregator = null;

            _ProductCatalogItems.Clear();
            _ProductCatalogItems = null;

            //close the order service client
            try
            {
                _InventoryServiceClient.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"the following error occured while trying to dispose the InventoryService {e.Message}");
            }
        }
        #endregion

        #region Delegates
        private delegate void ServiceInitializedDelegate();

        private ServiceInitializedDelegate _ServiceInitializedDelegate;

        protected void OnServiceInitialized()
        {
            this.IsServiceInitialized = true;

            //Get Product Catalog Items
            var productCatalog = _InventoryServiceClient.GetProductCatalog();
            foreach (InventoryServiceRef.ProductCatalogItem productCatalogItemRef in productCatalog.ProductsCatalogItems)
            {
                ProductCatalogItem productCatalogItem = new ProductCatalogItem()
                                                        {
                                                            Quantity = productCatalogItemRef.Quantity,
                                                            Product = new InventoryClient.Common.Models.Product()
                                                                      {
                                                                          Id = productCatalogItemRef.Product.Id,
                                                                          UnitPrice = productCatalogItemRef.Product.UnitPrice,
                                                                          Name = productCatalogItemRef.Product.Name
                                                                      }
                                                        };

                _ProductCatalogItems.Add(productCatalogItem.Product.Id, productCatalogItem);
                productCatalogItems.Add(productCatalogItem);
            }

            Debug.WriteLine($"InventoryService initialized");

            //Raise InventoryServiceInitializedEvent
            _EventAggregator.GetEvent<InventoryServiceInitializedEvent>().Publish("");
        }
        #endregion
    }
}
