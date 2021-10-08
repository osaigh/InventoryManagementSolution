using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using InventoryServiceLibrary;
using Xunit;
using Product = InventoryServiceLibrary.Product;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace InventoryServiceTest.IntegrationTests
{
    [Collection("ServiceHostFixtureCollection")]
    public class OrderServiceTests
    {
        #region Fields
        private readonly ServiceHostFixture _ServiceHostFixture;
        
        #endregion

        #region Properties

        #endregion

        #region Constructor

        public OrderServiceTests(ServiceHostFixture serviceHostFixture)
        {
            _ServiceHostFixture = serviceHostFixture;
        }
        #endregion

        #region Methods
        [Fact]
        public void CreateOrder_NoArgument_ReturnsNewOrder()
        {
            //Arrange
            var service = _ServiceHostFixture.GetOrderService();
            
            //Act
            var result = service.CreateOrder();

            //Assert
            Assert.IsNotNull(result);
        }

        [Fact]
        public void DeleteOrder_ValidOrderId_DeletesOrder()
        {
            //Arrange
            var service = _ServiceHostFixture.GetOrderService();

            //Act

            var order = service.CreateOrder();
            var result = service.DeleteOrder(order.Id);

            //Assert
            Assert.IsTrue(result);
        }

        [Fact]
        public void AddProductQuantityToOrder_QuantityLessThanStock_AddsProductDetailToOrder()
        {
            //Arrange
            var productCatalogItems = TestData.GetTestProductCatalogItemData();
            Product testProduct = productCatalogItems[0].Product;
            ProductCatalogItem testProductCatalogItem = productCatalogItems[0];
            Order testorder = TestData.GetTestOrder();

            var service = _ServiceHostFixture.GetOrderService();

            //Act
            var result = service.AddProductQuantityToOrder(testProduct.Id, testProductCatalogItem.Quantity-1, testorder.Id);

            //Assert
            Assert.IsTrue(result);
        }

        [Fact]
        public void AddProductQuantityToOrder_QuantityGreaterThanStock_AddsProductDetailToOrder()
        {
            //Arrange
            var productCatalogItems = TestData.GetTestProductCatalogItemData();
            Product testProduct = productCatalogItems[1].Product;
            ProductCatalogItem testProductCatalogItem = productCatalogItems[1];
            Order testorder = TestData.GetTestOrder();

            var service = _ServiceHostFixture.GetOrderService();

            //Act
            var result =service.AddProductQuantityToOrder(testProduct.Id, testProductCatalogItem.Quantity + 1, testorder.Id);

            //Assert
           Assert.IsFalse(result);
        }

        [Fact]
        public void RemoveProductFromOrder()
        {

            //Arrange
            var productCatalogItems = TestData.GetTestProductCatalogItemData();
            Product testProduct = productCatalogItems[2].Product;
            ProductCatalogItem testProductCatalogItem = productCatalogItems[2];
            Order testorder = TestData.GetTestOrder();

            var service = _ServiceHostFixture.GetOrderService();

            //Act
            var result = service.AddProductQuantityToOrder(testProduct.Id, testProductCatalogItem.Quantity - 1, testorder.Id);
            result = service.RemoveProductFromOrder(testProduct.Id, testorder.Id);

            //Assert
            Assert.IsTrue(result);
        }

        #endregion
    }
}
