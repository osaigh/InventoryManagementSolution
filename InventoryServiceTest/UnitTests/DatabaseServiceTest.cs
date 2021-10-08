using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryServiceLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace InventoryServiceTest.UnitTests
{
    [TestClass]
    public class DatabaseServiceTest
    {
        #region Fields

        #endregion 

        #region Properties

        #endregion 

        #region Constructor

        #endregion 

        #region Methods
        [Fact]
        public void GetProductCatalog_NoArgument_ReturnsProductCatalog()
        {
            //Arrange
            List<ProductCatalogItem> productCatalogItems = new List<ProductCatalogItem>();
            Product testProduct = new Product(){Id=Guid.NewGuid().ToString(),UnitPrice = 10.0, Name = "TestProduct"};
            ProductCatalogItem testProductCatalogItem = new ProductCatalogItem(){Quantity = 1,Product = testProduct };
            productCatalogItems.Add(testProductCatalogItem);

            productCatalogItems.Add(new ProductCatalogItem());
            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(dataStore => dataStore.GetProductCatalogItems()).Returns(() =>
                                                                                         {
                                                                                             return productCatalogItems;
                                                                                         });
           
            DatabaseService.Current.SetDataStore(mockDataStore.Object);

            //Act
            var productCatalog = DatabaseService.Current.GetProductCatalog();

            //reset
            DatabaseService.Current.SetDataStore(new DataStore());

            //Assert
            Assert.IsNotNull(productCatalog);
            Assert.AreEqual(testProductCatalogItem, productCatalog.ProductsCatalogItems.First());
            Assert.AreEqual(testProductCatalogItem.Product, productCatalog.ProductsCatalogItems.First().Product);
        }

        [Theory]
        [InlineData("123678","1",20,15)]
        public void AddProductQuantityToOrder_QuantityLessThanStock_AddsProductDetailToOrder(string productId, string orderId, int stock, int quantity)
        {
            //Arrange
            Product testProduct = new Product() { Id = productId, UnitPrice = 10.0, Name = "TestProduct" };
            ProductCatalogItem testProductCatalogItem = new ProductCatalogItem() { Quantity = stock, Product = testProduct };
            Order order = new Order(){Id = orderId, OrderDetails =  new List<OrderDetail>()};

            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(dataStore => dataStore.GetOrder(It.IsAny<string>())).Returns(() =>
                                                                                         {
                                                                                             return order;
                                                                                         });

            mockDataStore.Setup(dataStore => dataStore.GetProductCatalogItem(It.IsAny<string>()))
                         .Returns(() =>
                                  {
                                      return testProductCatalogItem;
                                  });


            DatabaseService.Current.SetDataStore(mockDataStore.Object);

            //Act
            DatabaseService.Current.AddProductQuantityToOrder(testProduct.Id, quantity, order.Id);

            //reset
            DatabaseService.Current.SetDataStore(new DataStore());

            //Assert
            Assert.AreEqual(1,order.OrderDetails.Count());
            Assert.AreEqual(productId, order.OrderDetails.First().ProductId);
            Assert.AreEqual(quantity, order.OrderDetails.First().Quantity);
            Assert.AreEqual(stock-quantity, testProductCatalogItem.Quantity);
        }

        [Theory]
        [InlineData("123678", "1", 10, 15)]
        public void AddProductQuantityToOrder_QuantityGreaterThanStock_AddsProductDetailToOrder(string productId, string orderId, int stock, int quantity)
        {
            //Arrange
            Product testProduct = new Product() { Id = productId, UnitPrice = 10.0, Name = "TestProduct" };
            ProductCatalogItem testProductCatalogItem = new ProductCatalogItem() { Quantity = stock, Product = testProduct };
            Order order = new Order() { Id = orderId, OrderDetails = new List<OrderDetail>() };

            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(dataStore => dataStore.GetOrder(It.IsAny<string>())).Returns(() =>
                                                                                             {
                                                                                                 return order;
                                                                                             });

            mockDataStore.Setup(dataStore => dataStore.GetProductCatalogItem(It.IsAny<string>()))
                         .Returns(() =>
                                  {
                                      return testProductCatalogItem;
                                  });


            DatabaseService.Current.SetDataStore(mockDataStore.Object);

            //Act
            DatabaseService.Current.AddProductQuantityToOrder(testProduct.Id, quantity, order.Id);

            //reset
            DatabaseService.Current.SetDataStore(new DataStore());

            //Assert
            Assert.AreEqual(0, order.OrderDetails.Count());
            Assert.AreEqual(stock, testProductCatalogItem.Quantity);
        }

        [Theory]
        [InlineData("123678", "1", 10, 15)]
        public void RemoveProductFromOrder(string productId, string orderId,int stock, int quantity)
        {
            //Arrange
            OrderDetail orderDetail = new OrderDetail()
                                      {
                                          ProductId = productId,
                                          Quantity = quantity
                                      };
            Order order = new Order() { Id = orderId, OrderDetails = new List<OrderDetail>(){ orderDetail } };

            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(dataStore => dataStore.GetOrder(It.IsAny<string>())).Returns(() =>
                                                                                             {
                                                                                                 return order;
                                                                                             });

            mockDataStore.Setup(dataStore => dataStore.GetOrder(It.IsAny<string>()))
                         .Returns(() =>
                                  {
                                      return order;
                                  });


            DatabaseService.Current.SetDataStore(mockDataStore.Object);

            //Act
            DatabaseService.Current.RemoveProductFromOrder(productId, order.Id);

            //reset
            DatabaseService.Current.SetDataStore(new DataStore());

            //Assert
            Assert.AreEqual(0, order.OrderDetails.Count());
        }

        public void CreateOrder()
        {
            //Arrange
            Order testOrder = new Order(){Id = "1", OrderDetails = new List<OrderDetail>()};

            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(dataStore => dataStore.CreateOrder()).Returns(() =>
                                                                                         {
                                                                                             return testOrder;
                                                                                         });

            DatabaseService.Current.SetDataStore(mockDataStore.Object);

            //Act
            var order = DatabaseService.Current.CreateOrder();

            //reset
            DatabaseService.Current.SetDataStore(new DataStore());

            //Assert
            Assert.AreEqual(testOrder, order);
        }

        [Theory]
        [InlineData("12", "12")]
        public void DeleteOrder_ValidOrderId_DeletesOrder(string testOrderId, string orderId)
        {
            //Arrange
            Order testOrder = new Order() { Id = testOrderId, OrderDetails = new List<OrderDetail>() };
            string deletedOrderId = null;

            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(dataStore => dataStore.DeleteOrder(It.IsAny<string>()))
                         .Callback((string id) =>
                                   {
                                       deletedOrderId = id;
                                   });

            DatabaseService.Current.SetDataStore(mockDataStore.Object);

            //Act
            DatabaseService.Current.DeleteOrder(orderId);

            //reset
            DatabaseService.Current.SetDataStore(new DataStore());

            //Assert
            Assert.AreEqual(testOrderId,deletedOrderId);
        }

        [Theory]
        [InlineData("12", "1")]
        public void DeleteOrder_InValidOrderId_DeletesOrder(string testOrderId, string orderId)
        {
            //Arrange
            Order testOrder = new Order() { Id = testOrderId, OrderDetails = new List<OrderDetail>() };
            string deletedOrderId = null;

            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(dataStore => dataStore.DeleteOrder(It.IsAny<string>()))
                         .Callback((string id) =>
                                   {
                                       deletedOrderId = id;
                                   });

            DatabaseService.Current.SetDataStore(mockDataStore.Object);

            //Act
            DatabaseService.Current.DeleteOrder(orderId);

            //reset
            DatabaseService.Current.SetDataStore(new DataStore());

            //Assert
            Assert.AreNotEqual(testOrderId,deletedOrderId);
        }
        #endregion 
    }
}
