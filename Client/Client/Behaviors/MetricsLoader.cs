using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Log;
using BrassLoon.Interface.Log.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class MetricsLoader : ICommand
    {
        private readonly IMetricService _metricService;
        private readonly ISettingsFactory _settingsFactory;
        private bool _canExecute = true;

        public MetricsLoader(IMetricService metricService, ISettingsFactory settingsFactory)
        {
            _metricService = metricService;
            _settingsFactory = settingsFactory;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainVM domainVM && !string.IsNullOrEmpty(domainVM.SelectedMetricEventCode))
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                domainVM.IsLoadingMetrics = true;
                domainVM.Metrics.Clear();
                DateTime metricsMaxTimestamp = DateTime.SpecifyKind(domainVM.MetricsMaxTimestamp, DateTimeKind.Local);
                Task.Run(() => _metricService.Search(_settingsFactory.CreateLogSettings(), domainVM.DomainId, metricsMaxTimestamp, domainVM.SelectedMetricEventCode).Result)
                    .ContinueWith(SearchCallback, domainVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task SearchCallback(Task<List<Metric>> search, object state)
        {
            try
            {
                List<Metric> metrics = await search;
                if (state is DomainVM domainVM)
                {
                    domainVM.Metrics.Clear();
                    foreach (Metric metric in metrics)
                    {
                        domainVM.Metrics.Add(new MetricVM(metric));
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _canExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
                if (state is DomainVM domainVM)
                    domainVM.IsLoadingMetrics = false;
            }
        }
    }
}
