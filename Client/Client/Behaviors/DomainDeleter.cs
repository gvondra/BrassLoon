using BrassLoon.Client.NavigationPage;
using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Models = BrassLoon.Interface.Account.Models;

namespace BrassLoon.Client.Behaviors
{
    public class DomainDeleter : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IDomainService _domainService;
        private readonly IAccountService _accountService;
        private readonly NavigationService _navigationService;
        private readonly bool _delete;
        private bool _canExecute = true;

        public DomainDeleter(ISettingsFactory settingsFactory, IDomainService domainService, IAccountService accountService)
            : this(settingsFactory, domainService, accountService, null, true)
        { }

        public DomainDeleter(
            ISettingsFactory settingsFactory,
            IDomainService domainService,
            IAccountService accountService,
            NavigationService navigationService,
            bool delete)
        {
            _settingsFactory = settingsFactory;
            _domainService = domainService;
            _accountService = accountService;
            _navigationService = navigationService;
            _delete = delete;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainVM domainVM && domainVM.InnerDomain.DomainId.HasValue)
            {
                if (!_delete || MessageBox.Show($"Are you sure you want to delete \"{domainVM.Name}\"", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _canExecute = false;
                    CanExecuteChanged.Invoke(this, new EventArgs());
                    _ = Task.Run(() => Delete(domainVM.InnerDomain))
                        .ContinueWith(DeleteCallback, domainVM, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            else if (parameter is AccountVM accountVM && accountVM.SelectedDeletedDomain != null)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                _ = Task.Run(() => Delete(accountVM.SelectedDeletedDomain.InnerDomain))
                        .ContinueWith(DeleteCallback, accountVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private Models.Domain Delete(Models.Domain domain)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "Deleted", _delete.ToString() }
            };
            return _domainService.UpdateDeleted(_settingsFactory.CreateAccountSettings(), domain.DomainId.Value, data).Result;
        }

        private async Task DeleteCallback(Task<Models.Domain> delete, object state)
        {
            try
            {
                Models.Domain domain = await delete;
                if (_navigationService != null)
                {
                    _ = Task.Run(() => GetAccount(domain.AccountId.Value))
                        .ContinueWith(GetAccountCallback, state, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
                _canExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }

        private Models.Account GetAccount(Guid accountId)
        => _accountService.Get(_settingsFactory.CreateAccountSettings(), accountId).Result;

        private async Task GetAccountCallback(Task<Models.Account> getAccount, object state)
        {
            try
            {
                Models.Account account = await getAccount;
                if (_navigationService != null)
                {
                    _ = _navigationService.Navigate(new Account(new AccountVM(account)));
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
                _canExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }
    }
}
