using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using InventoryServiceLibrary;

namespace InventoryServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServices();
        }

        public static void StartServices()
        {
            //BaseAddress
            Uri inventoryServiceBaseAddress = new Uri("http://localhost:8734/InventorySystem/");
            Uri orderServiceBaseAddress = new Uri("http://localhost:8733/InventorySystem/");

            //ServiceHost
            ServiceHost inventoryServiceHost = new ServiceHost(typeof(InventoryService), inventoryServiceBaseAddress);
            ServiceHost orderServiceHost = new ServiceHost(typeof(OrderService), orderServiceBaseAddress);

            try
            {
                //Service EndPoint
                inventoryServiceHost.AddServiceEndpoint(typeof(IInventoryService), new WSDualHttpBinding(), "InventoryService");
                orderServiceHost.AddServiceEndpoint(typeof(IOrderService), new WSHttpBinding(), "OrderService");

                //Service Behaviour
                ServiceMetadataBehavior inventoryServiceServiceMetadataBehavior = new ServiceMetadataBehavior();
                inventoryServiceServiceMetadataBehavior.HttpGetEnabled = true;
                inventoryServiceHost.Description.Behaviors.Add(inventoryServiceServiceMetadataBehavior);

                ServiceMetadataBehavior orderServiceServiceMetadataBehavior = new ServiceMetadataBehavior();
                orderServiceServiceMetadataBehavior.HttpGetEnabled = true;
                orderServiceHost.Description.Behaviors.Add(orderServiceServiceMetadataBehavior);

                inventoryServiceHost.Open();
                Console.WriteLine("Inventory Service Ready!");

                orderServiceHost.Open();
                Console.WriteLine("OrderService Service Ready!");

                Console.ReadLine();
                inventoryServiceHost.Close();
                orderServiceHost.Close();
            }
            catch (Exception e)
            {
                //log error
                Console.WriteLine("The following error occured: "+ e.Message);
            }
        }
    }
}
