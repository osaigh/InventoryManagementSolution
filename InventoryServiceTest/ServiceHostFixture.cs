using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryServiceLibrary;

namespace InventoryServiceTest
{
    public class ServiceHostFixture : IDisposable
    {
        #region Fields

        private static string _inventoryServiceURI = "http://localhost:8734/InventorySystem/InventoryService";
        private static string _orderServiceURI = "http://localhost:8733/InventorySystem/OrderService";
        private ServiceHost _inventoryServiceHost;
        private ServiceHost _orderServiceHost;
        private IInventoryService _inventoryService;
        private IOrderService _orderService;
        private IChannelFactory<IOrderService> _orderServicefactory;
        private IChannelFactory<IInventoryService> _inventoryServiceChannelFactory;
        private TestClientCallbackContract _testClientCallback;
        private bool stopCalled = false;
        #endregion

        #region Constructor
        public ServiceHostFixture()
        {
            Start();
        }
        #endregion

        #region Methods

        public void Start()
        {
            var task = new Task(() =>
                                {
                                    StartServices();
                                });
            task.Start();
            task.Wait();
            _testClientCallback = new TestClientCallbackContract();
            InitializeOrderService();
            InitializeInventoryService();
        }

        public IOrderService GetOrderService()
        {
            return _orderService;
        }

        public IInventoryService GetInventoryService()
        {
            return _inventoryService;
        }

        public TestClientCallbackContract GetClientCallback()
        {
            return _testClientCallback;
        }

        public EndpointAddress GetOrderServiceEndPointAddress()
        {
            EndpointAddress endpointAddress = new EndpointAddress(_orderServiceURI);
            return endpointAddress;
        }

        public EndpointAddress GetInventoryServiceEndPointAddress()
        {
            EndpointAddress endpointAddress = new EndpointAddress(_inventoryServiceURI);
            return endpointAddress;
        }

        #endregion

        #region Non Public Methods
        protected void StartServices()
        {
            //BaseAddress
            Uri inventoryServiceBaseAddress = new Uri("http://localhost:8734/InventorySystem/");
            Uri orderServiceBaseAddress = new Uri("http://localhost:8733/InventorySystem/");

            //ServiceHost
            _inventoryServiceHost = new ServiceHost(typeof(InventoryService), inventoryServiceBaseAddress);
            _orderServiceHost = new ServiceHost(typeof(OrderService), orderServiceBaseAddress);

            try
            {
                //Service EndPoint
                _inventoryServiceHost.AddServiceEndpoint(typeof(IInventoryService), new WSDualHttpBinding(), "InventoryService");
                _orderServiceHost.AddServiceEndpoint(typeof(IOrderService), new WSHttpBinding(), "OrderService");

                //Service Behaviour
                ServiceMetadataBehavior inventoryServiceServiceMetadataBehavior = new ServiceMetadataBehavior();
                inventoryServiceServiceMetadataBehavior.HttpGetEnabled = true;
                _inventoryServiceHost.Description.Behaviors.Add(inventoryServiceServiceMetadataBehavior);

                ServiceMetadataBehavior orderServiceServiceMetadataBehavior = new ServiceMetadataBehavior();
                orderServiceServiceMetadataBehavior.HttpGetEnabled = true;
                _orderServiceHost.Description.Behaviors.Add(orderServiceServiceMetadataBehavior);

                _inventoryServiceHost.Open();
                Debug.WriteLine("Inventory Service Ready!");

                _orderServiceHost.Open();
                Debug.WriteLine("OrderService Service Ready!");

            }
            catch (Exception e)
            {
                //log error
                Debug.WriteLine("The following error occured: " + e.Message);
            }
        }

        protected void InitializeOrderService()
        {
            var endpointaddress = this.GetOrderServiceEndPointAddress();
            var binding = new WSHttpBinding();
            _orderServicefactory =
                new ChannelFactory<IOrderService>(binding, endpointaddress);
            _orderService = _orderServicefactory.CreateChannel(endpointaddress);
        }

        protected void InitializeInventoryService()
        {
            var _inventoryServiceEndpointAddress = this.GetInventoryServiceEndPointAddress();
            var inventoryServiceBinding = new WSDualHttpBinding();
            _inventoryServiceChannelFactory =
                new DuplexChannelFactory<IInventoryService>(new InstanceContext(_testClientCallback), inventoryServiceBinding, _inventoryServiceEndpointAddress);
            _inventoryService = _inventoryServiceChannelFactory.CreateChannel(_inventoryServiceEndpointAddress);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            
            try
            {
                _orderServicefactory.Close();
                if (_orderServiceHost != null)
                {
                    _orderServiceHost.Close();
                }

                _inventoryService.UnsubscribeToProductQuantityChanged();
                _inventoryServiceChannelFactory.Close();
                if (_inventoryServiceHost != null)
                {
                    _inventoryServiceHost.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("The following error occured: " + e.Message);
            }
        }
        #endregion
    }
}
