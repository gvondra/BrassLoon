using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class UsersLoader : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IUserService _userService;
        private bool _canExecute = true;

        public UsersLoader(ISettingsFactory settingsFactory, IUserService userService)
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
                if (parameter is UsersVM usersVM)
                {
                    usersVM.Users.Clear();
                    usersVM.Message = string.Empty;
                    usersVM.SelectedUser = null;
                    _ = Task.Run(() => Search(usersVM.SearchText))
                        .ContinueWith(SearchCallback, usersVM, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
                _canExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }

        private List<User> Search(string emailAddress)
        {
            return _userService.Search(
                _settingsFactory.CreateAccountSettings(),
                emailAddress)
                .Result;
        }

        private async Task SearchCallback(Task<List<User>> search, object state)
        {
            try
            {
                List<User> users = await search;
                if (state is UsersVM usersVM)
                {
                    usersVM.Users.Clear();
                    foreach (User user in users)
                    {
                        usersVM.Users.Add(
                            new UserVM(user)
                            {
                                SaveCommand = new UserRoleSaver(_settingsFactory, _userService)
                            });
                    }
                    _ = Task.Run(() => GetRoles(users))
                        .ContinueWith(GetRolesCallback, state, TaskScheduler.FromCurrentSynchronizationContext());
                    if (usersVM.Users.Count == 0)
                        usersVM.Message = "No users found";
                    else if (usersVM.Users.Count == 1)
                        usersVM.SelectedUser = usersVM.Users[0];
                }
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

        private Dictionary<Guid, List<string>> GetRoles(List<User> users)
        {
            ISettings settings = _settingsFactory.CreateAccountSettings();
            Dictionary<Guid, List<string>> result = new Dictionary<Guid, List<string>>();
            foreach (User user in users)
            {
                result[user.UserId.Value] = _userService.GetRoles(settings, user.UserId.Value).Result;
            }
            return result;
        }

        private static async Task GetRolesCallback(Task<Dictionary<Guid, List<string>>> getRoles, object state)
        {
            try
            {
                Dictionary<Guid, List<string>> lookup = await getRoles;
                if (state is UsersVM usersVM)
                {
                    foreach (UserVM userVM in usersVM.Users)
                    {
                        if (lookup.ContainsKey(userVM.UserId))
                        {
                            userVM.IsSystemAdministrator = lookup[userVM.UserId].Contains("sysadmin");
                            userVM.IsAccountAdministrator = lookup[userVM.UserId].Contains("actadmin");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.Open(ex);
            }
        }
    }
}
