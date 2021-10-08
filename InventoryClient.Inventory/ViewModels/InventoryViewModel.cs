using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryClient.Common.Events;
using InventoryClient.Common.Models;
using InventoryClient.Common.Services;
using Prism.Events;
using Prism.Mvvm;

namespace InventoryClient.Inventory.ViewModels
{
    public class InventoryViewModel: BindableBase,IDisposable, IInventoryViewModel
    {
        #region Fields
        private IInventoryService _IInventoryService;
        private IEventAggregator _EventAggregator;
        #endregion

        #region Properties
        public ObservableCollection<ProductCatalogItem> ProductCatalogItems
        {
            get
            {
                if (_IInventoryService.IsServiceInitialized)
                {
                    return _IInventoryService.ProductCatalogItems;
                }
                else
                {
                    return new ObservableCollection<ProductCatalogItem>();
                }
            }
            private set {
                OnPropertyChanged(new PropertyChangedEventArgs("ProductCatalogItems"));
            }
        }
        #endregion

        #region Constructor

        public InventoryViewModel(IInventoryService inventoryService, IEventAggregator eventAggregator)
        {
            _IInventoryService = inventoryService;
            _EventAggregator = eventAggregator;
            Initialize();
        }
        #endregion

        #region Methods

        protected void Initialize()
        {
            _EventAggregator.GetEvent<InventoryServiceInitializedEvent>().Subscribe(OnInventoryServiceInitializedEvent);
            OnInventoryServiceInitializedEvent("");
        }


        #endregion

        #region Event Handlers

        protected void OnInventoryServiceInitializedEvent(string payload)
        {
            if (_IInventoryService.IsServiceInitialized)
            {
                //var _productCatalogItems = _IInventoryService.GetProductCatalog();
                //this.productCatalogItems.Clear();
                //foreach (ProductCatalogItem productCatalogItem in _productCatalogItems)
                //{
                //    productCatalogItems.Add(productCatalogItem);
                //}
                this.ProductCatalogItems = _IInventoryService.ProductCatalogItems;
            }
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            //unsubscribe to events
            _EventAggregator.GetEvent<InventoryServiceInitializedEvent>().Unsubscribe(OnInventoryServiceInitializedEvent);
            _EventAggregator = null;
        }
        #endregion
    }
}
