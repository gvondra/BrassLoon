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

namespace BrassLoon.Client.Control
{
    /// <summary>
    /// Interaction logic for DomainWorkGroups.xaml
    /// </summary>
    public partial class DomainWorkGroups : UserControl
    {
        public DomainWorkGroups()
        {
            InitializeComponent();
            this.Loaded += DomainWorkGroups_Loaded;
        }

        DomainVM DomainVM => this.DataContext is DomainVM domainVM ? domainVM : null;

        private void DomainWorkGroups_Loaded(object sender, RoutedEventArgs e)
        {
            DomainVM domainVM = this.DomainVM;
            if (domainVM != null && domainVM.WorkGroups == null && this.IsVisible)
            {
                using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
                domainVM.WorkGroups = new WorkGroupsVM(
                    domainVM,
                    NavigationService.GetNavigationService(this));
                if (domainVM.WorkGroups.Add == null)
                    domainVM.WorkGroups.Add = scope.Resolve<DomainWorkGroupAdd>();
                if (!domainVM.WorkGroups.ContainsBehavior<DomainWorkGroupLoader>())
                {
                    DomainWorkGroupLoader loader = scope.Resolve<Func<DomainVM, DomainWorkGroupLoader>>()(domainVM);
                    domainVM.WorkGroups.AddBehavior(loader);
                    loader.LoadWorkGroups();
                }
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is ListView listView && DomainVM?.WorkGroups?.NavigationService != null && DomainVM?.WorkGroups?.SelectedGroup != null)
                {
                    NavigationPage.WorkGroup page = new NavigationPage.WorkGroup(DomainVM.WorkGroups.SelectedGroup);
                    DomainVM.WorkGroups.NavigationService.Navigate(page);
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }
    }
}
