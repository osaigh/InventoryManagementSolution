using System.Collections.ObjectModel;
using System.Windows.Input;
using InventoryClient.Common.Models;

namespace InventoryClient.Orders.ViewModels
{
    public interface IOrdersViewModel
    {
        ObservableCollection<Order> Orders { get; }
        ICommand AddOrderCommand { get; set; }
    }
}