using Autofac;
using BrassLoon.Client.Behaviors;
using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
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
using Models = BrassLoon.Interface.Account.Models;

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
                accountLoader.LoadClients();
                if (AccessToken.Get.UserHasActAdminAccess())
                {
                    accountLoader.LoadDeletedDomains();
                    if (AccountVM.RestoreDeletedDomain == null)
                        AccountVM.RestoreDeletedDomain = scope.Resolve<Func<NavigationService, bool, DomainDeleter>>()(NavigationService.GetNavigationService(this), false);
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

        private void UserInvitation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is ListView listView)
                {
                    NavigationService navigationService = NavigationService.GetNavigationService(this);
                    Invitation page = new Invitation((UserInvitationVM)listView.SelectedItem);
                    navigationService.Navigate(page);
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        private void ClientsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is ListView listView && listView.SelectedItem != null)
                {
                    NavigationService navigationService = NavigationService.GetNavigationService(this);
                    navigationService.Navigate(new Client((ClientVM)listView.SelectedItem));
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        private void AddClientHyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Models.Client client = new Models.Client
                {
                    AccountId = AccountVM.AccountId,
                    Name = "New Client"
                };
                NavigationService navigationService = NavigationService.GetNavigationService(this);
                navigationService.Navigate(new Client(new ClientVM(client)));
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        private void AddDomainButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = (NewDomainName.Text ?? string.Empty).Trim();
                if (!string.IsNullOrEmpty(name))
                {
                    Models.Domain domain = new Models.Domain
                    {
                        AccountId = AccountVM.AccountId,
                        Name = name
                    };
                    NewDomainName.Text = string.Empty;
                    Task.Run(() => CreateDomain(domain))
                        .ContinueWith(CreateDomainCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        private Models.Domain CreateDomain(Models.Domain domain)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            ISettingsFactory settingsFactory = scope.Resolve<ISettingsFactory>();
            IDomainService domainService = scope.Resolve<IDomainService>();
            return domainService.Create(settingsFactory.CreateAccountSettings(), domain).Result;
        }

        private async Task CreateDomainCallback(Task<Models.Domain> createDomain, object state)
        {
            try
            {
                Models.Domain domain = await createDomain;
                DomainVM domainVM = new DomainVM(domain);
                AccountVM.Domains.Add(domainVM);
                NavigationService navigation = NavigationService.GetNavigationService(this);
                navigation.Navigate(new Domain(domainVM));
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }

        private void DomainListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is ListView listView && listView.SelectedItem != null)
                {
                    NavigationService navigation = NavigationService.GetNavigationService(this);
                    navigation.Navigate(new Domain((DomainVM)listView.SelectedItem));
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
