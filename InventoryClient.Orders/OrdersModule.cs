using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryClient.Common;
using InventoryClient.Orders.ViewModels;
using InventoryClient.Orders.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace InventoryClient.Orders
{
    public class OrdersModule : IModule
    {
        #region Constructor

        #endregion

        #region IModule
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = (IRegionManager)containerProvider.Resolve(typeof(IRegionManager));
            regionManager.AddToRegion(RegionNames.OrdersRegion, containerProvider.Resolve(typeof(OrdersView)));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IOrdersViewModel), typeof(OrdersViewModel));
            containerRegistry.Register(typeof(IOrderViewModel), typeof(OrderViewModel));
            containerRegistry.Register(typeof(OrderView), typeof(OrderView));
            containerRegistry.Register(typeof(OrdersView), typeof(OrdersView));
        }
        #endregion
    }
}
