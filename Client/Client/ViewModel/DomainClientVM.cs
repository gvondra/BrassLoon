using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Models = BrassLoon.Interface.Authorization.Models;

namespace BrassLoon.Client.ViewModel
{
    public class DomainClientVM : ViewModelBase
    {
        private readonly Models.Client _client;
        private readonly DomainVM _domainVM;
        private ICommand _generateSecret;
        private ICommand _save;

        public DomainClientVM(Models.Client client, DomainVM domainVM)
        {
            _client = client;
            _domainVM = domainVM;
        }

        internal Models.Client InnerClient => _client;

        internal DomainVM DomainVM => _domainVM;

        public ObservableCollection<AppliedRoleVM> AppliedRoles { get; } = new ObservableCollection<AppliedRoleVM>();

        public Guid? ClientId => _client.ClientId;

        public Guid? DomainId => _client.DomainId;

        public string Name
        {
            get => _client.Name;
            set
            {
                if (_client.Name != value)
                {
                    _client.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsActive
        {
            get => _client.IsActive ?? false;
            set
            {
                if (_client.IsActive != value)
                {
                    _client.IsActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string UserEmailAddress
        {
            get => _client.UserEmailAddress;
            set
            {
                if (_client.UserEmailAddress != value)
                {
                    _client.UserEmailAddress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string UserName
        {
            get => _client.UserName;
            set
            {
                if (_client.UserName != value)
                {
                    _client.UserName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Secret
        {
            get => _client.Secret;
            set
            {
                if (_client.Secret != value)
                {
                    _client.Secret = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsNew => !ClientId.HasValue;

        public ICommand GenerateSecret
        {
            get => _generateSecret;
            set
            {
                if (_generateSecret != value)
                {
                    _generateSecret = value;
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
