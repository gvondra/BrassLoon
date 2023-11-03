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
    /// Interaction logic for Domain.xaml
    /// </summary>
    public partial class Domain : Page
    {
        public Domain()
            : this(null)
        { }

        public Domain(DomainVM domainVM)
        {
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            InitializeComponent();
            DomainVM = domainVM;
            DataContext = domainVM;
            DomainUserSearchVM = domainVM != null ? new DomainUserSearchVM(domainVM) : null;
            this.Loaded += Domain_Loaded;
        }

        internal DomainVM DomainVM { get; private set; }
        internal DomainUserSearchVM DomainUserSearchVM 
        { 
            get => (DomainUserSearchVM)DomainUserSearch?.DataContext;
            private set => DomainUserSearch.DataContext = value;
        }

        private void Domain_Loaded(object sender, RoutedEventArgs e)
        {
            using ILifetimeScope scope = DependencyInjection.ContainerFactory.BeginLifetimeScope();
            if (DomainVM.Save == null)
                DomainVM.Save = scope.Resolve<DomainUpdater>();
            if (DomainVM.Delete == null)
                DomainVM.Delete = scope.Resolve<Func<NavigationService, bool, DomainDeleter>>()(NavigationService.GetNavigationService(this), true);
            if (DomainVM.ExceptionLoad == null)
                DomainVM.ExceptionLoad = scope.Resolve<ExceptionsLoader>();
            if (DomainVM.MoreExceptionLoad == null)
                DomainVM.MoreExceptionLoad = scope.Resolve<MoreExceptionsLoader>();
            if (DomainVM.MetricLoad == null)
                DomainVM.MetricLoad = scope.Resolve<MetricsLoader>();
            if (DomainVM.MoreMetricLoad == null)
                DomainVM.MoreMetricLoad = scope.Resolve<MoreMetricsLoader>();
            if (DomainVM.TraceLoad == null)
                DomainVM.TraceLoad = scope.Resolve<TracesLoader>();
            if (DomainVM.MoreTraceLoad == null)
                DomainVM.MoreTraceLoad = scope.Resolve<MoreTracesLoader>();
            if (DomainVM.RoleAdd == null)
                DomainVM.RoleAdd = scope.Resolve<DomainRoleAdd>();
            if (DomainVM.ClientAdd == null)
                DomainVM.ClientAdd = scope.Resolve<DomainClientAdd>();
            if (DomainUserSearchVM.Search == null)
                DomainUserSearchVM.Search = scope.Resolve<DomainUserSearcher>();
            if (DomainVM.SigningKeyAdd == null)
                DomainVM.SigningKeyAdd = scope.Resolve<DomainSigningKeyAdd>();
            if (!DomainVM.ContainsBehavior<DomainValidator>())
            {
                DomainVM.AddBehavior(
                    scope.Resolve<Func<DomainVM, DomainValidator>>()(DomainVM));
            }    
            if (!DomainVM.ContainsBehavior<DomainLoader>())
            {
                DomainLoader loader = scope.Resolve<Func<DomainVM, DomainLoader>>()(DomainVM);
                DomainVM.AddBehavior(loader);
                loader.LoadExceptions();
                loader.LoadTraceEventCodes();
                loader.LoadMetricEventCodes();
                loader.LoadItemCodes();
                loader.LoadLookupCodes();
                loader.LoadRoles();
                loader.LoadClients();
                loader.LoadSigningKeys();
            }
        }
    }
}
