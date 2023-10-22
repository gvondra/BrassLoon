using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Config;
using BrassLoon.Interface.Log;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogModels = BrassLoon.Interface.Log.Models;

namespace BrassLoon.Client.Behaviors
{
    public class DomainLoader
    {
        private readonly DomainVM _domainVM;
        private readonly IExceptionService _exceptionService;
        private readonly ITraceService _traceService;
        private readonly IMetricService _metricService;
        private readonly IItemService _itemService;
        private readonly ILookupService _lookupService;
        private readonly ISettingsFactory _settingsFactory;

        public DomainLoader(
            DomainVM domainVM,
            IExceptionService exceptionService,
            ITraceService traceService,
            IMetricService metricService,
            IItemService itemService,
            ILookupService lookupService,
            ISettingsFactory settingsFactory)
        {
            _domainVM = domainVM;
            _exceptionService = exceptionService;
            _traceService = traceService;
            _metricService = metricService;
            _itemService = itemService;
            _lookupService = lookupService;
            _settingsFactory = settingsFactory;
        }

        public void LoadExceptions()
        {
            _domainVM.IsLoadingExceptions = true;
            _domainVM.Exceptions.Clear();
            Task.Run(() => _exceptionService.Search(_settingsFactory.CreateLogSettings(), _domainVM.DomainId, _domainVM.ExceptionsMaxTimestamp.ToUniversalTime()))
                .ContinueWith(LoadExceptionsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadExceptionsCallback(Task<List<LogModels.Exception>> getExceptions, object state)
        {
            try
            {
                List<LogModels.Exception> exceptions = await getExceptions;
                _domainVM.Exceptions.Clear();
                foreach (LogModels.Exception exception in exceptions)
                {
                    _domainVM.Exceptions.Add(new ExceptionVM(exception));
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _domainVM.IsLoadingExceptions = false;
            }
        }

        public void LoadTraceEventCodes()
        {
            _domainVM.TraceEventCodes.Clear();
            Task.Run(() => _traceService.GetEventCodes(_settingsFactory.CreateLogSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadTraceEventCodesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadTraceEventCodesCallback(Task<List<string>> loadTraceEventCodes, object state)
        {
            try
            {
                _domainVM.TraceEventCodes.Clear();
                foreach (string code in await loadTraceEventCodes)
                {
                    _domainVM.TraceEventCodes.Add(code);
                }
                if (_domainVM.TraceEventCodes.Count > 0)
                {
                    _domainVM.SelectedTraceEventCode = _domainVM.TraceEventCodes[0];
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        public void LoadMetricEventCodes()
        {
            _domainVM.MetricEventCodes.Clear();
            Task.Run(() => _metricService.GetEventCodes(_settingsFactory.CreateLogSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadMetricEventCodesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadMetricEventCodesCallback(Task<List<string>> loadMetricEventCodes, object state)
        {
            try
            {
                _domainVM.MetricEventCodes.Clear();
                foreach (string code in await loadMetricEventCodes)
                {
                    _domainVM.MetricEventCodes.Add(code);
                }
                if (_domainVM.MetricEventCodes.Count > 0)
                    _domainVM.SelectedMetricEventCode = _domainVM.MetricEventCodes[0];
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        public void LoadItemCodes()
        {
            _domainVM.ItemCodes.Clear();
            Task.Run(() => _itemService.GetCodes(_settingsFactory.CreateConfigSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadItemCodesCodesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadItemCodesCodesCallback(Task<List<string>> loadMetricEventCodes, object state)
        {
            try
            {
                _domainVM.ItemCodes.Clear();
                foreach (string code in await loadMetricEventCodes)
                {
                    _domainVM.ItemCodes.Add(code);
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }

        public void LoadLookupCodes()
        {
            _domainVM.LookupCodes.Clear();
            Task.Run(() => _lookupService.GetCodes(_settingsFactory.CreateConfigSettings(), _domainVM.DomainId).Result)
                .ContinueWith(LoadLookupCodesCodesCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadLookupCodesCodesCallback(Task<List<string>> loadMetricEventCodes, object state)
        {
            try
            {
                _domainVM.LookupCodes.Clear();
                foreach (string code in await loadMetricEventCodes)
                {
                    _domainVM.LookupCodes.Add(code);
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }
    }
}
