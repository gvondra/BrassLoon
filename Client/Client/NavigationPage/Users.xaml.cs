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
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Page
    {
        public Users()
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            UsersVM = new UsersVM();
            DataContext = UsersVM;
            this.Loaded += Users_Loaded;
        }

        internal UsersVM UsersVM { get; set; }

        private void Users_Loaded(object sender, RoutedEventArgs e)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            if (UsersVM.SearchCommand == null)
                UsersVM.SearchCommand = scope.Resolve<UsersLoader>();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Hyperlink hyperlink)
                {
                    UsersVM.SelectedUser = (UserVM)hyperlink.DataContext;
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex, Window.GetWindow(this));
            }
        }
    }
}
