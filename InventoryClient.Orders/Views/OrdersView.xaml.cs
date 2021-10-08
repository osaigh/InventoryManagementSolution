using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InventoryClient.Orders.ViewModels;

namespace InventoryClient.Orders.Views
{
    /// <summary>
    /// Interaction logic for OrdersView.xaml
    /// </summary>
    public partial class OrdersView : UserControl
    {
        #region Properties
        public IOrdersViewModel Model
        {
            get; set;
        }

        #endregion
        public OrdersView(IOrdersViewModel ordersViewModel)
        {
            InitializeComponent();
            this.Model = ordersViewModel;
            this.DataContext = ordersViewModel;
        }
    }
}
