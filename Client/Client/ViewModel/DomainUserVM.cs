using BrassLoon.Interface.Authorization.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class DomainUserVM : ViewModelBase
    {
        private readonly User _user;
        private ICommand _save;

        public DomainUserVM(User user)
        {
            _user = user;
        }

        internal User InnerUser => _user;

        public ObservableCollection<AppliedRoleVM> AppliedRoles { get; } = new ObservableCollection<AppliedRoleVM>();

        public Guid? UserId => _user.UserId;

        public Guid? DomainId => _user.DomainId;

        public string ReferenceId => _user.ReferenceId;

        public string EmailAddress => _user.EmailAddress;

        public string Name
        {
            get => _user.Name;
            set
            {
                if (_user.Name != value)
                {
                    _user.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand Save
        {
            get => _save;
            set
            {
                if (_save != value)
                {
                    _save = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
