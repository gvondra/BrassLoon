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
    /// Interaction logic for DomainTaskTypes.xaml
    /// </summary>
    public partial class DomainTaskTypes : UserControl
    {
        public DomainTaskTypes()
        {
            InitializeComponent();
            this.Loaded += DomainTaskTypes_Loaded;
        }

        DomainVM DomainVM => this.DataContext is DomainVM domainVM ? domainVM : null;

        private void DomainTaskTypes_Loaded(object sender, RoutedEventArgs e)
        {
            DomainVM domainVM = this.DomainVM;
            if (domainVM != null && domainVM.TaskTypes == null && this.IsVisible)
            {
                using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
                domainVM.TaskTypes = new WorkTaskTypesVM(
                    domainVM,
                    NavigationService.GetNavigationService(this));
                if (domainVM.TaskTypes.Add == null)
                    domainVM.TaskTypes.Add = scope.Resolve<DomainTaskTypeAdd>();
                if (!domainVM.TaskTypes.ContainsBehavior<DomainTaskTypeLoader>())
                {
                    DomainTaskTypeLoader loader = scope.Resolve<Func<DomainVM, DomainTaskTypeLoader>>()(domainVM);
                    domainVM.TaskTypes.AddBehavior(loader);
                    loader.LoadWorkTaskTypes();
                }
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView && DomainVM?.TaskTypes?.NavigationService != null && DomainVM?.TaskTypes?.SelectedTaskType != null)
            {
                NavigationPage.TaskType taskType = new NavigationPage.TaskType();
                DomainVM.TaskTypes.NavigationService.Navigate(taskType);
            }
        }
    }
}
