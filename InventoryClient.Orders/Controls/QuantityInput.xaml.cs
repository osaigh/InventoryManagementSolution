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
using InventoryClient.Common.Models;

namespace InventoryClient.Orders.Controls
{
    /// <summary>
    /// Interaction logic for QuantityInput.xaml
    /// </summary>
    public partial class QuantityInput : UserControl
    {
        #region Properties

        public int MaxAllowableQuantity
        {
            get { return (int)GetValue(MaxAllowableQuantityProperty); }
            set { SetValue(MaxAllowableQuantityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxAllowableQuantity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxAllowableQuantityProperty =
            DependencyProperty.Register("MaxAllowableQuantity", typeof(int), typeof(QuantityInput), new PropertyMetadata(0));




        public int Quantity
        {
            get { return (int)GetValue(QuantityProperty); }
            set { SetValue(QuantityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Quantity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register("Quantity", typeof(int), typeof(QuantityInput), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(QuantityPropertyChanged),new CoerceValueCallback(CoerceQuantityProperty)));

        protected static void QuantityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuantityInput quantityInput = d as QuantityInput;
            
        }

        protected static object CoerceQuantityProperty(DependencyObject d, object value)
        {
            QuantityInput quantityInput = d as QuantityInput;
            int newValue = (int)value;
            //if value is invalid, just return default of 36
            if (newValue <= 0)
            {
                return 0;
            }
            //if value is odd, return default of 36
            if (newValue > quantityInput.MaxAllowableQuantity)
            {
                newValue = quantityInput.MaxAllowableQuantity;
                return newValue;
            }
            else
            {
                return value;
            }

        }
        #endregion

        #region Constructor
        public QuantityInput()
        {
            InitializeComponent();
            tb.DataContext = this;
        }
        #endregion
    }
}
