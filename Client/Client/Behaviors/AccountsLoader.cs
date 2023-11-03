using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class AccountsLoader : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IAccountService _accountService;
        private bool _canExecute = true;

        public AccountsLoader(ISettingsFactory settingsFactory, IAccountService accountService)
        {
            _settingsFactory = settingsFactory;
            _accountService = accountService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            _canExecute = false;
            CanExecuteChanged.Invoke(this, new EventArgs());
            try
            {
                if (parameter is AccountsVM accountsVM)
                {
                    accountsVM.Message = string.Empty;
                    accountsVM.Accounts.Clear();
                    Task.Run(() => Search(accountsVM.SearchText))
                        .ContinueWith(SearchCallback, accountsVM, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
                _canExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }

        private List<Account> Search(string emailAddress)
        {
            return _accountService.Search(_settingsFactory.CreateAccountSettings(), emailAddress).Result;
        }

        private async Task SearchCallback(Task<List<Account>> search, object state)
        {
            try
            {
                List<Account> accounts = await search;
                if (state is AccountsVM accountsVM)
                {
                    accountsVM.Accounts.Clear();
                    foreach (Account account in accounts)
                    {
                        accountsVM.Accounts.Add(new AccountVM(account));
                    }
                    if (accountsVM.Accounts.Count == 0)
                        accountsVM.Message = "No accounts found";
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
            }
        }
    }
}
