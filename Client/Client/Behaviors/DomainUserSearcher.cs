using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Authorization;
using BrassLoon.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainUserSearcher : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private bool _canExecute = true;

        public DomainUserSearcher(ISettingsFactory settingsFactory, IUserService userService, IRoleService roleService)
        {
            _settingsFactory = settingsFactory;
            _userService = userService;
            _roleService = roleService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainUserSearchVM searchVM && !string.IsNullOrEmpty(searchVM.SearchText))
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                searchVM.Users.Clear();
                searchVM.SelectedUser = null;
                _ = Task.Run(() =>
                {
                    List<User> users = _userService.Search(_settingsFactory.CreateAuthorizationSettings(), searchVM.DomainVM.DomainId, searchVM.SearchText).Result;
                    List<Role> roles = _roleService.GetByDomainId(_settingsFactory.CreateAuthorizationSettings(), searchVM.DomainVM.DomainId).Result;
                    return (dynamic)new
                    {
                        Users = users,
                        Roles = roles
                    };
                })
                    .ContinueWith(SearchCallback, searchVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task SearchCallback(Task<dynamic> search, object state)
        {
            try
            {
                dynamic values = await search;
                List<User> users = values.Users;
                List<Role> roles = values.Roles;
                if (state is DomainUserSearchVM searchVM)
                {
                    searchVM.Users.Clear();
                    searchVM.SelectedUser = null;
                    foreach (User user in users)
                    {
                        DomainUserVM userVM = new DomainUserVM(user);
                        foreach (Role role in roles)
                        {
                            AppliedRoleVM appliedRoleVM = new AppliedRoleVM(new AppliedRole { Name = role.Name, PolicyName = role.PolicyName });
                            appliedRoleVM.IsApplied = user.Roles.Exists(r => string.Equals(r.PolicyName, role.PolicyName, StringComparison.OrdinalIgnoreCase));
                            userVM.AppliedRoles.Add(appliedRoleVM);
                        }
                        userVM.AddBehavior(new DomainUserValidator(userVM));
                        userVM.Save = new DomainUserSaver(_settingsFactory, _userService);
                        searchVM.Users.Add(userVM);
                    }
                    if (searchVM.Users.Count > 0)
                    {
                        searchVM.SelectedUser = searchVM.Users[0];
                    }
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
    }
}
