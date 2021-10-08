using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryServiceLibrary;

namespace InventoryServiceTest
{
    public class TestClientCallbackContract : IClientCallbackContract
    {
        private int _quantity;
        private string _productId;
        public void ProductQuantityChanged(string productId, int quantity)
        {
            this._productId = productId;
            this._quantity = quantity;
        }

        public int GetQuantity()
        {
            return _quantity;
        }

        public string GetProductId()
        {
            return _productId;
        }
    }
}
