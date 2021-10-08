using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class InventoryServiceCallback : InventoryServiceRef.IInventoryServiceCallback
    {
        public void ProductQuantityChanged(string productId, int quantity)
        {
            Console.WriteLine("Product: "+ productId + " Quantity: "+ quantity);
        }
    }
}
