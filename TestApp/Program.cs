using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestApp.InventoryServiceRef;
using TestApp.OrderServiceRef;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            StartInventoryClient();
            StartOrderServiceClients();
            Console.ReadLine();
        }

        private static void StartOrderServiceClients()
        {
            Task.Run(() =>
                     {
                         OrderServiceClient client1 = new OrderServiceClient();
                         Random random = new Random((int)DateTime.Now.Ticks);

                         //create the order
                         Order order = client1.CreateOrder();
                         bool flag = false;
                         while (true)
                         {
                             int mins =  random.Next(1, 3) ;
                             Thread.Sleep(mins*1000);
                             
                             if (flag)
                             {
                                 Console.WriteLine("Add" + mins + " ProductId: " + 23000);
                                 client1.AddProductQuantityToOrder("23000", mins, order.Id);
                                 flag = false;
                             }
                             else
                             {
                                 Console.WriteLine("Remove ProductId: " + 23000);
                                 client1.RemoveProductFromOrder("23000", order.Id);
                                 flag = true;
                             }
                             
                         }

                         Console.WriteLine("Client 1 shutting down");
                     });

            Task.Run(() =>
                     {
                         OrderServiceClient client2 = new OrderServiceClient();
                         Random random = new Random((int)DateTime.Now.Ticks);

                         Order order = client2.CreateOrder();
                         bool flag = false;
                         while (true)
                         {
                             int mins = random.Next(1, 3);
                             Thread.Sleep(mins * 2000);

                             if (flag)
                             {
                                 Console.WriteLine("Add " + mins + " ProductId: " + 56723);
                                 client2.AddProductQuantityToOrder("56723", mins, order.Id);
                                 flag = false;
                             }
                             else
                             {
                                 Console.WriteLine("Remove ProductId: " + 56723);
                                 client2.RemoveProductFromOrder("56723", order.Id);
                                 flag = true;
                             }

                         }

                         Console.WriteLine("Client 1 shutting down");
                     });
        }

        private static void StartInventoryClient()
        {
            Task.Run(() =>
                     {
                         InventoryServiceCallback serviceCallback = new InventoryServiceCallback();
                         InventoryServiceClient client1 = new InventoryServiceClient(new InstanceContext(serviceCallback));

                         var t = client1.State;
                         //Subscribe.
                         Console.WriteLine("Subscribing");
                         client1.SubscribeToProductQuantityChanged();

                         while (true)
                         {

                         }

                         var ct = client1.GetProductCatalog();
                         

                         Console.WriteLine("Unsubscribing");
                         client1.SubscribeToProductQuantityChangedAsync();

                         //Closing the client gracefully closes the connection and cleans up resources
                         client1.Close();
                     });
            
        }
    }
}
