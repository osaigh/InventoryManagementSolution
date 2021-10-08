using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    public class ProductQuantityChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Id of the Product 
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// The new quantity of the Product
        /// </summary>
        public int Quantity { get; set; }
    }
}
