using BrassLoon.Client.NavigationPage;
using BrassLoon.Client.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace BrassLoon.Client.Control
{
    /// <summary>
    /// Interaction logic for AccountList.xaml
    /// </summary>
    public partial class AccountList : UserControl
    {
        public AccountList()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink)
            {
                Account page = new Account((AccountVM)hyperlink.DataContext);
                NavigationService navigationService = NavigationService.GetNavigationService(this);
                navigationService.Navigate(page);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService navigationService = NavigationService.GetNavigationService(this);
            navigationService.Navigate(new Uri("NavigationPage/NewAccount.xaml", UriKind.Relative));
        }
    }
}
