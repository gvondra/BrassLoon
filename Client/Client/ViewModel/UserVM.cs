using BrassLoon.Interface.Account.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class UserVM : ViewModelBase
    {
        private readonly User _user;
        private ICommand _saveCommand;
        private bool _isSysAdmin;
        private bool _isActAdmin;

        public UserVM(User user)
        {
            _user = user;
        }

        public Guid UserId => _user.UserId.Value;

        public string Name => _user.Name;

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

        public bool IsSystemAdministrator
        {
            get => _isSysAdmin;
            set
            {
                if (_isSysAdmin != value)
                {
                    _isSysAdmin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsAccountAdministrator
        {
            get => _isActAdmin;
            set
            {
                if ( _isActAdmin != value)
                {
                    _isActAdmin = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
