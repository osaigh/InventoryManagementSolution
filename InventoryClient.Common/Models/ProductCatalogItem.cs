using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace InventoryClient.Common.Models
{
    public class ProductCatalogItem : BindableBase
    {
        #region Properties
        private Product product = null;
        public Product Product
        {
            get { return product; }
            set
            {
                SetProperty(ref this.product, value, "Product");
            }
        }

        private int quantity = 0;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                SetProperty(ref this.quantity, value, "Quantity");
            }
        }

        #endregion
    }
}
