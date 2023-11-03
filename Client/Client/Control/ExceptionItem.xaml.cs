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

namespace BrassLoon.Client.Control
{
    /// <summary>
    /// Interaction logic for ExceptionItem.xaml
    /// </summary>
    public partial class ExceptionItem : UserControl
    {
        public ExceptionItem()
        {
            InitializeComponent();
            InitializeBindings();
        }

        public string ExceptionType
        {
            get
            {
                if (DataContext == null)
                    return string.Empty;
                else
                    return DataContext.GetType().FullName;
            }
        }

        public static readonly DependencyProperty BoundDataContextProperty = DependencyProperty.Register(
            "BoundDataContextProperty",
            typeof(object),
            typeof(ExceptionItem),
            new PropertyMetadata(null, OnBoundDataContextPropertyChanged)
            );

        public static void OnBoundDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExceptionItem)d).ExceptionTypeText.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
        }

        private void InitializeBindings()
        {
            this.SetBinding(BoundDataContextProperty, new Binding());
            Binding binding = new Binding("ExceptionType");
            binding.Source = this;
            ExceptionTypeText.SetBinding(TextBlock.TextProperty, binding);
        }
    }
}
