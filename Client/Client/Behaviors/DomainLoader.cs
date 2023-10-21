using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Log;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogModels = BrassLoon.Interface.Log.Models;

namespace BrassLoon.Client.Behaviors
{
    public class DomainLoader
    {
        private readonly DomainVM _domainVM;
        private readonly IExceptionService _exceptionService;
        private readonly ISettingsFactory _settingsFactory;

        public DomainLoader(
            DomainVM domainVM,
            IExceptionService exceptionService,
            ISettingsFactory settingsFactory)
        {
            _domainVM = domainVM;
            _exceptionService = exceptionService;
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
    }
}
