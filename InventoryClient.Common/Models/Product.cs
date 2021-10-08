using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace InventoryClient.Common.Models
{
    public class Product : BindableBase
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

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                SetProperty(ref this.name, value, "Name");
            }
        }

        private double unitPrice = 0;
        public double UnitPrice
        {
            get { return unitPrice; }
            set
            {
                SetProperty(ref this.unitPrice, value, "UnitPrice");
            }
        }

        #endregion
    }
}
