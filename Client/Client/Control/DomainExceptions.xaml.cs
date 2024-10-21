using BrassLoon.Client.NavigationPage;
using BrassLoon.Client.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BrassLoon.Client.Control
{
    /// <summary>
    /// Interaction logic for DomainExceptions.xaml
    /// </summary>
    public partial class DomainExceptions : UserControl
    {
        public DomainExceptions()
        {
            InitializeComponent();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is ListView listView && listView.SelectedItem != null)
                {
                    NavigationService navigationService = NavigationService.GetNavigationService(this);
                    navigationService.Navigate(new ExceptionDetails((ExceptionVM)listView.SelectedItem));
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
