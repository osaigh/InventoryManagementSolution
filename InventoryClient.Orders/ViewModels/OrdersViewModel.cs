using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using InventoryClient.Common.Events;
using InventoryClient.Common.Models;
using InventoryClient.Common.Services;
using InventoryClient.Orders.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;

namespace InventoryClient.Orders.ViewModels
{
    public class OrdersViewModel : BindableBase, IDisposable, IOrdersViewModel
    {
        #region Fields
        private IEventAggregator _EventAggregator;
        private IContainerProvider _ContainerProvider;
        #endregion

        #region Properties
        private ObservableCollection<Order> orders = new ObservableCollection<Order>();

        public ObservableCollection<Order> Orders
        {
            get { return orders; }
            private set
            {
                SetProperty(ref this.orders, value, "Orders");
            }
        }

        public ICommand AddOrderCommand
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public OrdersViewModel(IEventAggregator eventAggregator, IContainerProvider containerProvider)
        {
            _EventAggregator = eventAggregator;
            _ContainerProvider = containerProvider;
            Initialize();
        }
        #endregion

        #region Methods

        protected void Initialize()
        {
            _EventAggregator.GetEvent<OrderCreatedEvent>().Subscribe(OnOrderCreatedEvent);
            this.AddOrderCommand = new DelegateCommand(OnAddOrderCommandExecute);
        }

        protected void OnAddOrderCommandExecute()
        {
            var orderView = _ContainerProvider.Resolve<OrderView>();
            orderView.Show();
        }
        #endregion

        #region Event Handlers

        protected void OnOrderCreatedEvent(Order order)
        {
            if (order != null)
            {
                orders.Add(order);
            }
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            orders.Clear();
            orders = null;

            //unsubscribe to events
            _EventAggregator.GetEvent<OrderCreatedEvent>().Unsubscribe(OnOrderCreatedEvent);
            _EventAggregator = null;
        }
        #endregion
    }
}
