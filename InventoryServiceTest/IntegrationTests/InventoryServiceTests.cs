using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryServiceLibrary;
using Xunit;
using Product = InventoryServiceLibrary.Product;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace InventoryServiceTest.IntegrationTests
{
    [Collection("ServiceHostFixtureCollection")]
    public class InventoryServiceTests
    {
        #region Fields
        private readonly ServiceHostFixture _ServiceHostFixture;

        #endregion

        #region Properties

        #endregion

        #region Constructor

        public InventoryServiceTests(ServiceHostFixture serviceHostFixture)
        {
            _ServiceHostFixture = serviceHostFixture;
        }
        #endregion

        #region Methods
        [Fact]
        public void GetProductCatalog_NoArgument_ReturnsProductCatalog()
        {
            //Arrange
            var service = _ServiceHostFixture.GetInventoryService();

            //Act
            var result = service.GetProductCatalog();

            //Assert
            Assert.IsNotNull(result);
        }

        [Fact]
        public async Task AddProductQuantityToOrder_QuantityLessThanStock_AddsProductDetailToOrder()
        {
            //Arrange
            var productCatalogItems = TestData.GetTestProductCatalogItemData();
            Product testProduct = productCatalogItems[3].Product;
            ProductCatalogItem testProductCatalogItem = productCatalogItems[3];
            Order testorder = TestData.GetTestOrder();

            //callback
            var testClientCallback = _ServiceHostFixture.GetClientCallback();

            //OrderService
            var orderService = _ServiceHostFixture.GetOrderService();

            //InventoryService
            var inventoryService = _ServiceHostFixture.GetInventoryService();

            //Act
            //subscribe
            inventoryService.SubscribeToProductQuantityChanged();

            //add quantity
            var order = orderService.CreateOrder();
            orderService.AddProductQuantityToOrder(testProduct.Id, testProductCatalogItem.Quantity - 1, order.Id);
            
            //we pause execution for a few seconds to wait for the clientcallback to receive
            // a callback message from the service. 
            Task task = new Task(() =>
                                 {
                                     int spincycles = 0;
                                     while (testClientCallback.GetProductId() == null || testClientCallback.GetProductId() != testProduct.Id)
                                     {
                                         if (spincycles == 10)
                                         {
                                             // break out after 10 sleep cycles
                                             break;
                                         }
                                         Thread.Sleep(3000);
                                         spincycles++;
                                     }

                                 });
            task.Start();
            await task;
            
            string receivedProductId = testClientCallback.GetProductId(); //MessageBox.Show(receivedProductId);
            int receivedQuantity = testClientCallback.GetQuantity();
            

            //Assert
            Assert.AreEqual(testProduct.Id, receivedProductId);
            Assert.IsTrue(receivedQuantity > 0);
        }

        #endregion
    }
}
