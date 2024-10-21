using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BrassLoon.Client.Control
{
    /// <summary>
    /// Interaction logic for ExceptionItem.xaml
    /// </summary>
    public partial class ExceptionItem : UserControl
    {
        public static readonly DependencyProperty BoundDataContextProperty = DependencyProperty.Register(
            "BoundDataContextProperty",
            typeof(object),
            typeof(ExceptionItem),
            new PropertyMetadata(null, OnBoundDataContextPropertyChanged));

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

        public static void OnBoundDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((ExceptionItem)d).ExceptionTypeText.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();

        private void InitializeBindings()
        {
            _ = SetBinding(BoundDataContextProperty, new Binding());
            Binding binding = new Binding("ExceptionType");
            binding.Source = this;
            _ = ExceptionTypeText.SetBinding(TextBlock.TextProperty, binding);
        }
    }
}
