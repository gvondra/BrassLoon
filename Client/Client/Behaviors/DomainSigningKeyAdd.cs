using BrassLoon.Client.ViewModel;
using BrassLoon.Interface.Authorization.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class DomainSigningKeyAdd : ICommand
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
                DomainSigningKeyVM signingKeyVM = new DomainSigningKeyVM(
                    new SigningKey() { IsActive = true, DomainId = domainVM.DomainId },
                    domainVM);
                domainVM.SigningKeys.Add(signingKeyVM);
                domainVM.SelectedSigningKey = signingKeyVM;
            }
        }
    }
}
