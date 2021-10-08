using System.Collections.ObjectModel;
using InventoryClient.Common.Models;

namespace InventoryClient.Inventory.ViewModels
{
    public interface IInventoryViewModel
    {
        ObservableCollection<ProductCatalogItem> ProductCatalogItems { get; }
    }
}