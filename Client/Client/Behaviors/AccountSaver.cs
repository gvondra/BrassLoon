using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class AccountSaver : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IAccountService _accountService;
        private bool _canExcecute = true;

        public AccountSaver(ISettingsFactory settingsFactory, IAccountService accountService)
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
                    Task.Run(() => Save(accountVM.InnerAccount))
                        .ContinueWith(SaveCallback, accountVM, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
                _canExcecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }

        private Account Save(Account account)
        {
            return _accountService.Update(_settingsFactory.CreateAccountSettings(), account).Result;
        }

        private async Task SaveCallback(Task<Account> save, object state)
        {
            try
            {
                await save;
                MessageBox.Show("Account Saved", "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
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
