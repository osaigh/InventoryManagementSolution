using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using InventoryClient.Common.Events;
using InventoryClient.Wpf.OrderServiceRef;
using Polly;
using Polly.Retry;
using Prism.Events;
using IOrderService = InventoryClient.Common.Services.IOrderService;
using Order = InventoryClient.Common.Models.Order;
using OrderDetail = InventoryClient.Common.Models.OrderDetail;

namespace InventoryClient.Wpf.Services
{
    public class OrderService : IOrderService, INotifyPropertyChanged, IDisposable
    {
        #region Fields
        private IEventAggregator _EventAggregator;
        private OrderServiceRef.OrderServiceClient _OrderServiceClient = null;
        private static int RETRY_ATTEMPTS = 10;
        #endregion

        #region Properties

        private bool isReady = false;

        public bool IsReady
        {
            get { return isReady; }
            private set
            {
                isReady = value;
                OnPropertyChanged("IsReady");
            }
        }
        #endregion

        #region Constructor

        public OrderService(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the OrderService
        /// </summary>
        private void Initialize()
        {
            Debug.WriteLine($"OrderService initializing");
            _EventAggregator.GetEvent<InventoryServiceInitializedEvent>().Subscribe(OnInventoryServiceInitializedEvent);
            _ServiceInitializedDelegate = new ServiceInitializedDelegate(OnServiceInitialized);
            StartClient();
        }

        /// <summary>
        /// Starts the OrderServiceClient
        /// </summary>
        protected void StartClient()
        {
            _OrderServiceClient = new OrderServiceClient();
            Task.Run(() =>
                     {
                         var policy = RetryPolicy.Handle<SocketException>()
                                                 .Or<System.ServiceModel.EndpointNotFoundException>()
                                                 .Or<Exception>()
                                                 .WaitAndRetry(RETRY_ATTEMPTS, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                                               (ex, time) =>
                                                               {
                                                                   //Log error
                                                                   Debug.WriteLine($"The following error occured {ex.Message}");
                                                               });

                         policy.Execute(() =>
                                        {
                                            Thread.Sleep(1000);
                                            if (_OrderServiceClient.State != CommunicationState.Created && _OrderServiceClient.State != CommunicationState.Opened)
                                            {
                                                throw new Exception($"OrderServiceClient is in the {_OrderServiceClient.State}");
                                            }
                                        });

                         if (_OrderServiceClient.State == CommunicationState.Created)
                         {
                             System.Windows.Application.Current.Dispatcher.Invoke(_ServiceInitializedDelegate);
                         }
                         else
                         {
                             Debug.WriteLine("OrderServiceClient unable to connect to server in a timely manner. App will shutdown");
                         }
                     });
        }
        #endregion

        #region Event Handlers

        protected void OnInventoryServiceInitializedEvent(string payload)
        {
            if (IsReady)
            {
                return;
            }
            else
            {
                if (_OrderServiceClient.State != CommunicationState.Opened)
                {
                    try
                    {
                        _OrderServiceClient.Close();
                        StartClient();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"the following error occured while trying to start the OrderService {e.Message}");
                    }
                }
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
            //unsubscribe from events
            _EventAggregator.GetEvent<InventoryServiceInitializedEvent>().Unsubscribe(OnInventoryServiceInitializedEvent);
            _EventAggregator = null;

            //close the order service client
            try
            {
                _OrderServiceClient.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"the following error occured while trying to dispose the OrderService {e.Message}");
            }
        }
        #endregion

        #region IOrderService
        public bool AddProductQuantityToOrder(string productId, int quantity, string orderId)
        {
            bool result = _OrderServiceClient.AddProductQuantityToOrder(productId, quantity, orderId);

            return result;
        }

        public Order CreateOrder()
        {
            var orderRef = _OrderServiceClient.CreateOrder();

            var order = new Order()
                        {
                            Id = orderRef.Id,
                            OrderDetails = new ObservableCollection<OrderDetail>()
                        };
            return order;
        }

        public bool DeleteOrder(string orderId)
        {
            bool result = _OrderServiceClient.DeleteOrder(orderId);

            return result;
        }

        public bool RemoveProductFromOrder(string productId, string orderId)
        {
            bool result = _OrderServiceClient.RemoveProductFromOrder(productId,orderId);

            return result;
        }

        public bool SaveOrder(Order order)
        {
            var orderRef = new OrderServiceRef.Order()
                           {
                               Id = order.Id
                           };
            List<OrderServiceRef.OrderDetail> orderDetails = new List<OrderServiceRef.OrderDetail>();
            foreach (OrderDetail orderDetail in order.OrderDetails)
            {
                OrderServiceRef.OrderDetail orderDetailRef = new OrderServiceRef.OrderDetail()
                                                             {
                                                                 ProductId = orderDetail.ProductId,
                                                                 Quantity = orderDetail.Quantity
                                                             };
                orderDetails.Add(orderDetailRef);
            }

            orderRef.OrderDetails = orderDetails.ToArray();
            var result = _OrderServiceClient.SaveOrder(orderRef);

            return result;
        }
        #endregion

        #region Delegates
        private delegate void ServiceInitializedDelegate();

        private ServiceInitializedDelegate _ServiceInitializedDelegate;

        protected void OnServiceInitialized()
        {
            this.IsReady = true;

            Debug.WriteLine($"OrderService initialized");
        }
        #endregion
    }
}
