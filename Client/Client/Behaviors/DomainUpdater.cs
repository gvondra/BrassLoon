using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Models = BrassLoon.Interface.Account.Models;

namespace BrassLoon.Client.Behaviors
{
    public class DomainUpdater : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IDomainService _domainService;
        private bool _canExecute = true;

        public DomainUpdater(ISettingsFactory settingsFactory, IDomainService domainService)
        {
            _settingsFactory = settingsFactory;
            _domainService = domainService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainVM domainVM && !domainVM.HasErrors)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() => _domainService.Update(_settingsFactory.CreateAccountSettings(), domainVM.InnerDomain).Result)
                    .ContinueWith(UpdateCallback, domainVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task UpdateCallback(Task<Models.Domain> update, object state)
        {
            try
            {
                await update;
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _canExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }
    }
}
