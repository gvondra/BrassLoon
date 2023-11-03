using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Authorization.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainRoleAdd : ICommand
    {
        private bool _canExecute = true;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (parameter is DomainVM domainVM)
            {
                RoleVM roleVM = new RoleVM(new Role() { DomainId = domainVM.DomainId }, domainVM)
                {
                    IsActive = true,
                    Name = "New Role",
                    PolicyName = "policy:code"
                };
                domainVM.Roles.Add(roleVM);
                domainVM.SelectedRole = roleVM;
            }
        }
    }
}
