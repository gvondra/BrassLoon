using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Log;
using BrassLoon.Interface.Log.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class TracesLoader : ICommand
    {
        private readonly ITraceService _traceService;
        private readonly ISettingsFactory _settingsFactory;
        private bool _canExecute = true;

        public TracesLoader(ITraceService traceService, ISettingsFactory settingsFactory)
        {
            _traceService = traceService;
            _settingsFactory = settingsFactory;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainVM domainVM && !string.IsNullOrEmpty(domainVM.SelectedTraceEventCode))
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                domainVM.IsLoadingTraces = true;
                domainVM.Traces.Clear();
                Task.Run(() => _traceService.Search(_settingsFactory.CreateLogSettings(), domainVM.DomainId, domainVM.TracesMaxTimestamp, domainVM.SelectedTraceEventCode).Result)
                    .ContinueWith(SearchCallback, domainVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task SearchCallback(Task<List<Trace>> search, object state)
        {
            try
            {
                List<Trace> traces = await search;
                if (state is DomainVM domainVM)
                {
                    domainVM.Traces.Clear();
                    foreach (Trace trace in traces)
                    {
                        domainVM.Traces.Add(new TraceVM(trace));
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
                    domainVM.IsLoadingTraces = false;
            }
        }
    }
}
