using Autofac;
using BrassLoon.Client.Behaviors;
using BrassLoon.Client.ViewModel;
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

namespace BrassLoon.Client.NavigationPage
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Page
    {
        public Account()
            : this(null)
        { }

        internal Account(AccountVM accountVM)
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            AccountVM = accountVM;
            DataContext = accountVM;
            this.Loaded += Account_Loaded;
        }

        internal AccountVM AccountVM { get; private set; }

        private void Account_Loaded(object sender, RoutedEventArgs e)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            AccountLoader accountLoader = AccountVM.GetBehavior<AccountLoader>();
            if (accountLoader == null)
            {
                accountLoader = scope.Resolve<Func<AccountVM, AccountLoader>>()(AccountVM);
                AccountVM.AddBehavior(accountLoader);
                accountLoader.LoadDomains();
                if (AccessToken.Get.UserHasActAdminAccess())
                {
                    accountLoader.LoadDeletedDomains();
                    accountLoader.LoadUsers();
                    AccountVM.AccountUserRemover = scope.Resolve<AccountUserRemover>();
                }
            }
            accountLoader.LoadInvitations();
            if (AccountVM.GetBehavior<AccountValidator>() == null)
                AccountVM.AddBehavior(new AccountValidator(AccountVM));
            if (AccountVM.SaveCommand == null)
                AccountVM.SaveCommand = scope.Resolve<AccountSaver>();
            if (AccessToken.Get.UserHasActAdminAccess() && AccountVM.LockToggleCommand == null)
                AccountVM.LockToggleCommand = scope.Resolve<AccountLockToggler>();
            AccountVM.AdminVisibility = AccessToken.Get.UserHasActAdminAccess() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CreateInvitation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService navigationService = NavigationService.GetNavigationService(this);
                CreateInvitation page = new CreateInvitation(AccountVM);
                navigationService.Navigate(page);

            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
