using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Authorization;
using BrassLoon.Interface.Authorization.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainUserSaver : ICommand
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly IUserService _userService;
        private bool _canExecute = true;

        public DomainUserSaver(ISettingsFactory settingsFactory, IUserService userService)
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
            if (parameter is DomainUserVM userVM && !userVM.HasErrors)
            {
                _canExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                Task.Run(() =>
                {
                    User user = userVM.InnerUser;
                    user.Roles = userVM.AppliedRoles
                        .Where(r => r.IsApplied)
                        .Select<AppliedRoleVM, AppliedRole>(r => new AppliedRole { Name = r.Name, PolicyName = r.PolicyName })
                        .ToList();
                    return _userService.Update(_settingsFactory.CreateAuthorizationSettings(), user).Result;
                })
                    .ContinueWith(SaveCallback, userVM, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task SaveCallback(Task<User> save, object state)
        {
            try
            {
                await save;
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
