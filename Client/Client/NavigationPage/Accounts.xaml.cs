using Autofac;
using BrassLoon.Client.Behaviors;
using BrassLoon.Client.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BrassLoon.Client.NavigationPage
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class Accounts : Page
    {
        public Accounts()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            this.AccountsVM = new AccountsVM();
            this.DataContext = this.AccountsVM;
            this.Loaded += Accounts_Loaded;
        }

        internal AccountsVM AccountsVM { get; private set; }

        private void Accounts_Loaded(object sender, RoutedEventArgs e)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            if (AccountsVM.SearchCommand == null)
                AccountsVM.SearchCommand = scope.Resolve<AccountsLoader>();
            SearchText.Focus();
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
    }
}
