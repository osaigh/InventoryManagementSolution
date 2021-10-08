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
using InventoryClient.Inventory.ViewModels;

namespace InventoryClient.Inventory.Views
{
    /// <summary>
    /// Interaction logic for InventoryView.xaml
    /// </summary>
    public partial class InventoryView : UserControl
    {
        #region Properties
        public IInventoryViewModel Model
        {
            get; set;
        }

        #endregion
        public InventoryView(IInventoryViewModel inventoryViewModel)
        {
            InitializeComponent();
            this.Model = inventoryViewModel;
            this.DataContext = inventoryViewModel;
        }
    }
}
