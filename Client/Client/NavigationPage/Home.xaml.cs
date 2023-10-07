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
            this.Loaded += Home_Loaded;
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            GoogleLogin.ShowLoginDialog(owner: Window.GetWindow(this));
            HomeVM.SystemAdminVisibility =  AccessToken.Get.UserHasSysAdminAccess() ? Visibility.Visible : Visibility.Collapsed;
            HomeVM.AccountAdminVisibility = AccessToken.Get.UserHasActAdminAccess() ? Visibility.Visible : Visibility.Collapsed;
        }

        internal HomeVM HomeVM { get; set; }
    }
}
