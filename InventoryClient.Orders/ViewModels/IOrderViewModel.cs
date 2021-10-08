using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using InventoryClient.Common.Models;

namespace InventoryClient.Orders.ViewModels
{
    public interface IOrderViewModel
    {
        bool IsEnabled { get; }
        ObservableCollection<ProductCatalogItem> ProductCatalogItems { get; }
        ProductCatalogItem SelectedProductCatalogItem { get; set; }
        int SelectedProductCatalogItemQuantity { get; set; }
        int MaxQuantity { get; set; }
        ObservableCollection<OrderDetail> OrderDetails { get; }
        ICommand AddOrderDetailCommand { get; set; }
        ICommand SaveCommand { get; set; }
        void SetWindow(Window window);
    }
}