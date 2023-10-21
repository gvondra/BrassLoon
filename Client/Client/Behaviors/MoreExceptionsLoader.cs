using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Log;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Models = BrassLoon.Interface.Log.Models;

namespace BrassLoon.Client.Behaviors
{
    public class MoreExceptionsLoader : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IExceptionService _exceptionService;
        private bool _canExecute = true;

        public MoreExceptionsLoader(ISettingsFactory settingsFactory, IExceptionService exceptionService)
        {
            _settingsFactory = settingsFactory;
            _exceptionService = exceptionService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainVM domainVM)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                if (domainVM.Exceptions.Count > 0)
                {
                    domainVM.IsLoadingExceptions = true;
                    DateTime maxTimestamp = domainVM.Exceptions[domainVM.Exceptions.Count - 1].CreateTimestamp.Value.ToUniversalTime();
                    Task.Run(() => _exceptionService.Search(_settingsFactory.CreateLogSettings(), domainVM.DomainId, maxTimestamp))
                        .ContinueWith(SearchCallback, domainVM, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private async Task SearchCallback(Task<List<Models.Exception>> search, object state)
        {
            try
            {
                List<Models.Exception> exceptions = await search;
                if (exceptions.Count > 0 && state is DomainVM domainVM)
                {
                    foreach (Models.Exception exception in exceptions)
                    {
                        domainVM.Exceptions.Add(new ExceptionVM(exception));
                    }
                    _canExecute = true;
                    CanExecuteChanged.Invoke(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
                _canExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
            finally
            {
                if (state is DomainVM domainVM)
                    domainVM.IsLoadingExceptions = false;
            }
        }
    }
}
