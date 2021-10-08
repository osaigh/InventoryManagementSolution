using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryClient.Common;
using InventoryClient.Inventory.ViewModels;
using InventoryClient.Inventory.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace InventoryClient.Inventory
{
    public class InventoryModule : IModule
    {
        #region Constructor

        #endregion

        #region IModule
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = (IRegionManager)containerProvider.Resolve(typeof(IRegionManager));
            regionManager.AddToRegion(RegionNames.InventoryRegion, containerProvider.Resolve(typeof(InventoryView)));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(IInventoryViewModel), typeof(InventoryViewModel));
        }
        #endregion
    }
}
