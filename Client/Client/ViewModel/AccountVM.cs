using BrassLoon.Interface.Account.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    internal class AccountVM : ViewModelBase
    {
        private readonly Account _account;
        private ICommand _saveCommand;
        private ICommand _lockToggleCommand;
        private Visibility _adminVisibility = Visibility.Collapsed;

        public AccountVM(Account account)
        {
            _account = account;
        }

        internal Account InnerAccount => _account;

        public Guid AccountId => _account.AccountId.Value;

        public Visibility AdminVisibility
        {
            get => _adminVisibility;
            set
            {
                if (_adminVisibility != value)
                {
                    _adminVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand LockToggleCommand
        {
            get => _lockToggleCommand;
            set
            {
                if (_lockToggleCommand != value)
                {
                    _lockToggleCommand = value;
                    NotifyPropertyChanged();
                }
            }
        }

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
        public bool IsLocked
        {
            get => _account.Locked;
            set
            {
                if (_account.Locked != value)
                {
                    _account.Locked = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(LockButtonText));
                }
            }
        }

        public string LockButtonText => IsLocked ? "Unlock Account" : "Lock Account";
    }
}
