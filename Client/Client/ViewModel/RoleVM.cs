using BrassLoon.Interface.Authorization.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class RoleVM : ViewModelBase
    {
        private readonly Role _role;
        private readonly DomainVM _domainVM;
        private ICommand _save;

        public RoleVM(Role role, DomainVM domainVM)
        {
            _role = role;
            _domainVM = domainVM;
        }

        internal DomainVM DomainVM => _domainVM;

        internal Role InnerRole => _role;

        public Guid? RoleId => _role.RoleId;

        public Guid? DomainId => _role.DomainId;

        public string Name
        {
            get => _role.Name;
            set
            {
                if (_role.Name != value)
                {
                    _role.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string PolicyName
        {
            get => _role.PolicyName;
            set
            {
                if (_role.PolicyName != value)
                {
                    _role.PolicyName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool? IsActive
        {
            get => _role.IsActive;
            set
            {
                if (_role.IsActive != value)
                {
                    _role.IsActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Comment
        {
            get => _role.Comment;
            set
            {
                if (_role.Comment != value)
                {
                    _role.Comment = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsNew => !RoleId.HasValue;

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
