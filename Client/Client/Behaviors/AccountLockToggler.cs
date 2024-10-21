using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class AccountLockToggler : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IAccountService _accountService;
        private bool _canExcecute = true;

        public AccountLockToggler(ISettingsFactory settingsFactory, IAccountService accountService)
        {
            _settingsFactory = settingsFactory;
            _accountService = accountService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExcecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            try
            {
                if (parameter is AccountVM accountVM && !accountVM.HasErrors)
                {
                    _canExcecute = false;
                    CanExecuteChanged.Invoke(this, new EventArgs());
                    string messageText = string.Format(CultureInfo.InvariantCulture, "Are you sure you want to {0} this account?", accountVM.IsLocked ? "unlock" : "lock");
                    if (MessageBox.Show(messageText, "Verfify Toggle Action", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        _ = Task.Run(() => UpdateLock(accountVM.AccountId, !accountVM.IsLocked))
                            .ContinueWith(UpdateLockCallback, accountVM, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    else
                    {
                        _canExcecute = true;
                        CanExecuteChanged.Invoke(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
                _canExcecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }

        private bool UpdateLock(Guid accountId, bool isLocked)
        {
            Dictionary<string, string> patch = new Dictionary<string, string>()
            {
                { "Locked", isLocked.ToString() }
            };
            _ = _accountService.Patch(_settingsFactory.CreateAccountSettings(), accountId, patch);
            return isLocked;
        }

        private async Task UpdateLockCallback(Task<bool> updateLock, object state)
        {
            try
            {
                bool isLocked = await updateLock;
                if (state is AccountVM accountVM)
                {
                    accountVM.IsLocked = isLocked;
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                _canExcecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }
    }
}
