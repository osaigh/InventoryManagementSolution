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
using System.Windows.Shapes;
using InventoryClient.Orders.ViewModels;

namespace InventoryClient.Orders.Views
{
    /// <summary>
    /// Interaction logic for OrderView.xaml
    /// </summary>
    public partial class OrderView : Window
    {
        #region Properties
        public IOrderViewModel Model
        {
            get; set;
        }

        #endregion
        public OrderView(IOrderViewModel orderViewModel)
        {
            InitializeComponent();
            this.Model = orderViewModel;
            this.DataContext = orderViewModel;
            orderViewModel.SetWindow(this);
        }
    }
}
