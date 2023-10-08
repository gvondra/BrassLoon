using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class UserRoleSaver : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IUserService _userService;
        private bool _canExecute = true;

        public UserRoleSaver(ISettingsFactory settingsFactory, IUserService userService)
        {
            _settingsFactory = settingsFactory;
            _userService = userService;
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
                if (parameter is UserVM userVM)
                {
                    Task.Run(() => SaveRoles(userVM))
                        .ContinueWith(SaveRolesCallback, userVM, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
                _canExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }

        private void SaveRoles(UserVM userVM)
        {
            List<string> roles = new List<string>();
            if (userVM.IsSystemAdministrator)
                roles.Add("sysadmin");
            if (userVM.IsAccountAdministrator)
                roles.Add("actadmin");
            _userService.SaveRoles(_settingsFactory.CreateAccountSettings(), userVM.UserId, roles).Wait();
        }

        private async Task SaveRolesCallback(Task saveRoles, object state)
        {
            try
            {
                await saveRoles;
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
