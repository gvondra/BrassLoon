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
            this.Loaded += Domain_Loaded;
        }

        internal DomainVM DomainVM { get; private set; }

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
            if (DomainVM.GetBehavior<DomainValidator>() == null)
            {
                DomainVM.AddBehavior(
                    scope.Resolve<Func<DomainVM, DomainValidator>>()(DomainVM));
            }    
            if (DomainVM.GetBehavior<DomainLoader>() == null)
            {
                DomainLoader loader = scope.Resolve<Func<DomainVM, DomainLoader>>()(DomainVM);
                DomainVM.AddBehavior(loader);
                loader.LoadExceptions();
                loader.LoadTraceEventCodes();
                loader.LoadMetricEventCodes();
                loader.LoadItemCodes();
                loader.LoadLookupCodes();
            }
        }
    }
}
