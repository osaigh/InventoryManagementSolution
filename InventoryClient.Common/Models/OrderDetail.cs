using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace InventoryClient.Common.Models
{
    public class OrderDetail: BindableBase
    {
        #region Properties
        private string productId = string.Empty;
        public string ProductId
        {
            get { return productId; }
            set
            {
                SetProperty(ref this.productId, value, "ProductId");
            }
        }

        private string productName = string.Empty;
        public string ProductName
        {
            get { return productName; }
            set
            {
                SetProperty(ref this.productName, value, "ProductName");
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
