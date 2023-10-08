using BrassLoon.Interface.Account.Models;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    internal class AccountVM : ViewModelBase
    {
        private readonly Account _account;
        private ICommand _saveCommand;

        public AccountVM(Account account)
        {
            _account = account;
        }

        internal Account InnerAccount => _account;

        public ICommand SaveCommand
        {
            get => _saveCommand;
            set
            {
                if (_saveCommand != value)
                {
                    _saveCommand = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _account.Name;
            set
            {
                if (_account.Name != value)
                {
                    _account.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsLocked => _account.Locked;
    }
}
