using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using InventoryClient.Common.Services;
using InventoryClient.Inventory;
using InventoryClient.Orders;
using InventoryClient.Wpf.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;

namespace InventoryClient.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml. I am using Prism's PrismApplication which extends Application
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //Register modules that will be loaded on startup
            moduleCatalog.AddModule<InventoryModule>();
            moduleCatalog.AddModule<OrdersModule>();
        }

        protected override Window CreateShell()
        {
            //Register the shell
            return (Window)(Container.Resolve(typeof(ShellWindow)));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Register services here
            containerRegistry.RegisterSingleton(typeof(IInventoryService), typeof(InventoryService));
            containerRegistry.RegisterScoped(typeof(IOrderService), typeof(OrderService));
        }
    }
}
