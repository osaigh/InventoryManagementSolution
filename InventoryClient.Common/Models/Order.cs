using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace InventoryClient.Common.Models
{
    public class Order : BindableBase
    {
        #region Properties
        private string id = string.Empty;
        public string Id
        {
            get { return id; }
            set
            {
                SetProperty(ref this.id, value, "Id");
            }
        }

        private string customerId = string.Empty;
        public string CustomerId
        {
            get { return customerId; }
            set
            {
                SetProperty(ref this.customerId, value, "CustomerId");
            }
        }

        private ObservableCollection<OrderDetail> orderDetails = new ObservableCollection<OrderDetail>();
        public ObservableCollection<OrderDetail> OrderDetails
        {
            get { return orderDetails; }
            set
            {
                SetProperty(ref this.orderDetails, value, "OrderDetails");
            }
        }

        #endregion
    }
}
