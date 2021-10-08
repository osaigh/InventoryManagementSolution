using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryClient.Common.Models;

namespace InventoryClient.Common.Services
{
    public interface IInventoryService
    {
        bool IsServiceInitialized { get; }
        ObservableCollection<ProductCatalogItem> ProductCatalogItems { get; }
        IEnumerable<ProductCatalogItem> GetProductCatalog();
    }
}
