using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Authorization;
using BrassLoon.Interface.Authorization.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainRoleSaver : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IRoleService _roleService;
        private bool _canExecute = true;

        public DomainRoleSaver(ISettingsFactory settingsFactory, IRoleService roleService)
        {
            _settingsFactory = settingsFactory;
            _roleService = roleService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is RoleVM roleVM && !roleVM.HasErrors)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() =>
                {
                    Func<ISettings, Role, Task<Role>> save = _roleService.Update;
                    if (roleVM.IsNew)
                        save = _roleService.Create;
                    return save(_settingsFactory.CreateAuthorizationSettings(), roleVM.InnerRole).Result;
                })
                    .ContinueWith(SaveCallback, roleVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task SaveCallback(Task<Role> save, object state)
        {
            try
            {
                Role role = await save;
                if (state is RoleVM roleVM && roleVM.IsNew)
                {
                    int index = roleVM.DomainVM.Roles.IndexOf(roleVM);
                    if (index >= 0)
                    {
                        RoleVM newRoleVM = new RoleVM(role, roleVM.DomainVM);
                        roleVM.DomainVM.Roles[index] = newRoleVM;
                        roleVM.DomainVM.SelectedRole = newRoleVM;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorWindow.Open(ex);
            }
            finally
            {
                if (CanExecuteChanged != null)
                {
                    _canExecute = true;
                    CanExecuteChanged.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
