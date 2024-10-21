using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Client.Behaviors
{
    internal sealed class HomeLoader
    {
        private readonly HomeVM _homeVM;
        private readonly IAccountService _accountService;
        private readonly ISettingsFactory _settingsFactory;

        public HomeLoader(
            ISettingsFactory settingsFactory,
            IAccountService accountService,
            HomeVM homeVM)
        {
            _settingsFactory = settingsFactory;
            _accountService = accountService;
            _homeVM = homeVM;
        }

        public void LoadAccounts()
        {
            _homeVM.Accounts.Clear();
            _ = Task.Run(() => _accountService.Search(_settingsFactory.CreateAccountSettings()).Result)
                .ContinueWith(LoadAccountsCallback, null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task LoadAccountsCallback(Task<List<Account>> loadAccounts, object state)
        {
            try
            {
                List<Account> accounts = await loadAccounts;
                _homeVM.Accounts.Clear();
                foreach (Account account in accounts)
                {
                    _homeVM.Accounts.Add(new AccountVM(account));
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }
    }
}
