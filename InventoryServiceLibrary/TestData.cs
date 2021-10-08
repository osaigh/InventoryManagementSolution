using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServiceLibrary
{
    public class TestData
    {
        private static Order order = new Order()
                              {
                                  Id = "123",
                                  OrderDetails = new List<OrderDetail>()
                              };
        public static List<ProductCatalogItem> GetTestProductCatalogItemData()
        {
            List<ProductCatalogItem> productCatalogItems = new List<ProductCatalogItem>();

            //EngineeringMaths book
            ProductCatalogItem productCatalogItem1 = new ProductCatalogItem()
                                                     {
                                                         Quantity = 40,
                                                         Product = new Product()
                                                                   {
                                                                       Name = "StroudEngineering",
                                                                       UnitPrice = 99.99,
                                                                       Id = "11238"
                                                                   }
                                                     };

            //Cook book
            ProductCatalogItem productCatalogItem2 = new ProductCatalogItem()
                                                     {
                                                         Quantity = 30,
                                                         Product = new Product()
                                                                   {
                                                                       Name = "Recipes",
                                                                       UnitPrice = 10.0,
                                                                       Id = "23831"
                                                                   }
                                                     };

            //CSharp book
            ProductCatalogItem productCatalogItem3 = new ProductCatalogItem()
                                                     {
                                                         Quantity = 40,
                                                         Product = new Product()
                                                                   {
                                                                       Name = "CSharp Intro",
                                                                       UnitPrice = 59.99,
                                                                       Id = "56723"
                                                                   }
                                                     };

            //Time magazine
            ProductCatalogItem productCatalogItem4 = new ProductCatalogItem()
                                                     {
                                                         Quantity = 20,
                                                         Product = new Product()
                                                                   {
                                                                       Name = "Time Magazine",
                                                                       UnitPrice = 19.99,
                                                                       Id = "23000"
                                                                   }
                                                     };

            productCatalogItems.Add(productCatalogItem1);
            productCatalogItems.Add(productCatalogItem2);
            productCatalogItems.Add(productCatalogItem3);
            productCatalogItems.Add(productCatalogItem4);

            return productCatalogItems;
        }

        public static Order GetTestOrder()
        {
            return order;
        }
    }
}
