using BrassLoon.Client.ViewModel;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.Behaviors
{
    public class ExceptionsLoader : ICommand
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
                DomainLoader loader = domainVM.GetBehavior<DomainLoader>();
                if (loader != null)
                {
                    loader.LoadExceptions();
                }
            }
        }
    }
}
