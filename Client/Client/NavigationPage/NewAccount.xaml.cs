using Autofac;
using BrassLoon.Client.Behaviors;
using BrassLoon.Client.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Models = BrassLoon.Interface.Account.Models;

namespace BrassLoon.Client.NavigationPage
{
    /// <summary>
    /// Interaction logic for NewAccount.xaml
    /// </summary>
    public partial class NewAccount : Page
    {
        public NewAccount()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            DataContext = null;
            Loaded += NewAccount_Loaded;
        }

        internal AccountVM AccountVM { get; private set; }

        private void NewAccount_Loaded(object sender, RoutedEventArgs e)
        {
            Models.Account account = new Models.Account();
            AccountVM = new AccountVM(account)
            {
                Name = "new-account"
            };
            DataContext = AccountVM;
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            AccountVM.AddBehavior(new AccountValidator(AccountVM));
            AccountVM.SaveCommand = scope.Resolve<Func<bool, Action, AccountSaver>>()(
                true,
                () => NavigationService.Navigate(new Uri("NavigationPage/Home.xaml", UriKind.Relative)));
        }
    }
}
