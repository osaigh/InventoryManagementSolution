using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InventoryClient.Common.Events;
using InventoryClient.Common.Models;
using InventoryClient.Common.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace InventoryClient.Orders.ViewModels
{
    public class OrderViewModel: BindableBase, IDisposable,IOrderViewModel
    {
        #region Fields
        private IEventAggregator _EventAggregator;
        private IOrderService _OrderService = null;
        private IInventoryService _InventoryService = null;
        private bool stopPolling = false;
        private Order order = null;
        private Window _window;
        #endregion

        #region Properties
        private bool isEnabled = false;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { 
                SetProperty(ref this.isEnabled, value, "IsEnabled");
            }
        }

        private int maxQuantity = 0;

        public int MaxQuantity
        {
            get
            {
                return maxQuantity;
            }
            set
            {
                SetProperty(ref this.maxQuantity, value, "MaxQuantity");
            }
        }

        public ObservableCollection<ProductCatalogItem> ProductCatalogItems
        {
            get
            {
                if (_InventoryService.IsServiceInitialized)
                {
                    return _InventoryService.ProductCatalogItems;
                }
                else
                {
                    return new ObservableCollection<ProductCatalogItem>();
                }
            }
            private set
            {
                OnPropertyChanged(new PropertyChangedEventArgs("ProductCatalogItems"));
            }
        }


        private ProductCatalogItem selectedProductCatalogItem = null;
        public ProductCatalogItem SelectedProductCatalogItem
        {
            get
            {
                return selectedProductCatalogItem;
            }
            set
            {
                SetProperty(ref this.selectedProductCatalogItem, value, "SelectedProductCatalogItem");
                if (selectedProductCatalogItem != null)
                {
                    this.MaxQuantity = selectedProductCatalogItem.Quantity;
                }
                this.SelectedProductCatalogItemQuantity = 0;
            }
        }


        private int selectedProductCatalogItemQuantity = 0;
        public int SelectedProductCatalogItemQuantity
        {
            get { return selectedProductCatalogItemQuantity; }
            set
            {
                SetProperty(ref this.selectedProductCatalogItemQuantity, value, "SelectedProductCatalogItemQuantity");
            }
        }

        public ObservableCollection<OrderDetail> OrderDetails
        {
            get
            {
                if (order != null)
                {
                    return order.OrderDetails;
                }
                else
                {
                    return new ObservableCollection<OrderDetail>();
                }
            }
            set { OnPropertyChanged(new PropertyChangedEventArgs("OrderDetails")); }
        }

        public ICommand AddOrderDetailCommand
        {
            get;
            set;
        }

        public ICommand SaveCommand
        {
            get;
            set;
        }


        private string error = string.Empty;
        public string Error
        {
            get { return error; }
            set
            {
                SetProperty(ref this.error,value, "Error");
            }
        }
        #endregion

        #region Constructor
        public OrderViewModel(IOrderService orderService,IInventoryService inventoryService,IEventAggregator eventAggregator)
        {
            _OrderService = orderService;
            _EventAggregator = eventAggregator;
            _InventoryService = inventoryService;
            Initialize();
        }
        #endregion

        #region Methods

        protected void Initialize()
        {
            _OrderServiceReadyDelegate = new OrderServiceReadyDelegate(OnOrderServiceReady);
            AddOrderDetailCommand = new DelegateCommand(OnAddOrderDetailCommandExecute);
            SaveCommand = new DelegateCommand(OnSaveOrderExecute);
            PollOrderService();
        }

        /// <summary>
        /// Polls the OrderService until it is ready
        /// </summary>
        protected void PollOrderService()
        {
            Task.Run(() =>
                     {
                         while (!_OrderService.IsReady && !stopPolling)
                         {
                             Thread.Sleep(1000);
                         }

                         System.Windows.Application.Current.Dispatcher.Invoke(_OrderServiceReadyDelegate);
                     });

        }

        /// <summary>
        /// Execute method for the AddOrderDetailCommand
        /// </summary>
        protected void OnAddOrderDetailCommandExecute()
        {
            //Clear any Error
            Error = string.Empty;

            //validate Order details before creating
            if (SelectedProductCatalogItem != null && order != null)
            {
                if (SelectedProductCatalogItemQuantity > 0 && SelectedProductCatalogItemQuantity <= SelectedProductCatalogItem.Quantity)
                {
                    //check to see if this item has been added already
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        if (string.Compare(orderDetail.ProductId, selectedProductCatalogItem.Product.Id, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            return;
                        }
                    }

                    //Add to Order
                    bool result = false;
                    try
                    {
                        result = _OrderService.AddProductQuantityToOrder(SelectedProductCatalogItem.Product.Id, SelectedProductCatalogItemQuantity, order.Id);
                    }
                    catch (Exception e)
                    {
                        this.Error = $"The following error occured while trying to add product to order : {e.Message}";
                        Debug.WriteLine(this.Error);
                    }

                    if (result)
                    {
                        OrderDetail _orderDetail = new OrderDetail()
                                                   {
                                                       Quantity = SelectedProductCatalogItemQuantity,
                                                       ProductId = SelectedProductCatalogItem.Product.Id,
                                                       ProductName = SelectedProductCatalogItem.Product.Name
                                                   };
                        order.OrderDetails.Add(_orderDetail);
                        this.OnPropertyChanged(new PropertyChangedEventArgs("OrderDetails"));
                    }
                    
                }
            }
        }

        /// <summary>
        /// Execute method for the SaveOrderCommand
        /// </summary>
        protected void OnSaveOrderExecute()
        {
            //save order
            var result = false;

            try
            {
                result = _OrderService.SaveOrder(order);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"The following error occured while trying to save the order: {e.Message}");
            }

            //Raise event
            if (result)
            {
                _EventAggregator.GetEvent<OrderCreatedEvent>().Publish(order);
            }

            order = null;

            //close
            if (_window != null)
            {
                _window.Close();
            }

        }

        /// <summary>
        /// Sets the window
        /// </summary>
        /// <param name="window"></param>
        public void SetWindow(Window window)
        {
            _window = window;
            this._window.Closing += _window_Closing;
        }

        #endregion

        #region Event Handlers
        private void _window_Closing(object sender, CancelEventArgs e)
        {
            //delete the order before closing
            try
            {
                if (order != null)
                {
                    _OrderService.DeleteOrder(order.Id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"The following error occured while trying to remove the order: {ex.Message}");
            }

            Dispose();
        }
        #endregion

        #region Delegates
        private delegate void OrderServiceReadyDelegate();

        private OrderServiceReadyDelegate _OrderServiceReadyDelegate;

        protected void OnOrderServiceReady()
        {
            stopPolling = true;
            this.ProductCatalogItems = _InventoryService.ProductCatalogItems;
            order = _OrderService.CreateOrder();
            this.IsEnabled = true;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            stopPolling = true;
            _EventAggregator = null;
            order = null;
            _OrderService = null;
            _InventoryService = null;
        }

        #endregion
    }
}
