using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class AccountUserRemover : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IAccountService _accountService;
        private bool _canExcecute = true;

        public AccountUserRemover(ISettingsFactory settingsFactory, IAccountService accountService)
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
            if (parameter is AccountVM accountVM && accountVM.SelectedUser != null)
            {
                if (MessageBox.Show($"Are you sure you want to remove {accountVM.SelectedUser.Name}?", "Confirm Remove", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _canExcecute = false;
                    CanExecuteChanged.Invoke(this, new EventArgs());
                    try
                    {
                        Task.Run(() => RemoveUser(accountVM.AccountId, accountVM.SelectedUser.UserId))
                            .ContinueWith(RemoveUserCallback, accountVM, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    catch (System.Exception ex)
                    {
                        ErrorWindow.Open(ex);
                        _canExcecute = true;
                        CanExecuteChanged.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        private void RemoveUser(Guid accountId, Guid userId)
        {
            _accountService.DeleteUser(_settingsFactory.CreateAccountSettings(), accountId, userId).Wait();
        }

        private async Task RemoveUserCallback(Task removeUser, object state)
        {
            try
            {
                await removeUser;
                if (state != null && state is AccountVM accountVM)
                {
                    UserVM user = accountVM.SelectedUser;
                    accountVM.SelectedUser = null;
                    accountVM.Users.Remove(user);
                }
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
