using Autofac;
using BrassLoon.Client.Behaviors;
using BrassLoon.Client.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrassLoon.Client.NavigationPage
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            HomeVM = new HomeVM();
            DataContext = HomeVM;
            Loaded += Home_Loaded;
        }

        internal HomeVM HomeVM { get; set; }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            GoogleLogin.ShowLoginDialog(owner: Window.GetWindow(this));
            HomeVM.SystemAdminVisibility = AccessToken.Get.UserHasSysAdminAccess() ? Visibility.Visible : Visibility.Collapsed;
            HomeVM.AccountAdminVisibility = AccessToken.Get.UserHasActAdminAccess() ? Visibility.Visible : Visibility.Collapsed;
            HomeLoader homeLoader = HomeVM.GetBehavior<HomeLoader>() ?? scope.Resolve<Func<HomeVM, HomeLoader>>()(HomeVM);
            homeLoader.LoadAccounts();
        }
    }
}
