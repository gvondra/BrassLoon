using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class AccountVM : ViewModelBase
    {
        private readonly Account _account;
        private ICommand _saveCommand;
        private ICommand _lockToggleCommand;
        private Visibility _adminVisibility = Visibility.Collapsed;
        private ICommand _accountUserRemover;
        private ICommand _restoreDeletedDomain;
        private DomainVM _selectedDeletedDomain;
        private UserVM _selectedUser;

        public AccountVM(Account account)
        {
            _account = account;
            DeletedDomains.CollectionChanged += DeletedDomains_CollectionChanged;
        }

        internal Account InnerAccount => _account;

        public ObservableCollection<DomainVM> Domains { get; } = new ObservableCollection<DomainVM>();
        public ObservableCollection<DomainVM> DeletedDomains { get; } = new ObservableCollection<DomainVM>();
        public ObservableCollection<UserVM> Users { get; } = new ObservableCollection<UserVM>();
        public ObservableCollection<UserInvitationVM> Invitations { get; } = new ObservableCollection<UserInvitationVM>();
        public ObservableCollection<ClientVM> Clients { get; } = new ObservableCollection<ClientVM>();

        public Guid AccountId => _account.AccountId.Value;

        public UserVM SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand AccountUserRemover
        {
            get => _accountUserRemover;
            set
            {
                if (_accountUserRemover != value)
                {
                    _accountUserRemover = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Visibility AdminVisibility
        {
            get => _adminVisibility;
            set
            {
                if (_adminVisibility != value)
                {
                    _adminVisibility = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(DeletedDomainVisibility));
                }
            }
        }

        public Visibility DeletedDomainVisibility
        {
            get
            {
                if (AdminVisibility == Visibility.Collapsed)
                    return AdminVisibility;
                else
                    return DeletedDomains.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
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

        public ICommand RestoreDeletedDomain
        {
            get => _restoreDeletedDomain;
            set
            {
                if (_restoreDeletedDomain != value)
                {
                    _restoreDeletedDomain = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DomainVM SelectedDeletedDomain
        {
            get => _selectedDeletedDomain;
            set
            {
                if (_selectedDeletedDomain != value)
                {
                    _selectedDeletedDomain = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string LockButtonText => IsLocked ? "Unlock Account" : "Lock Account";

        private void DeletedDomains_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            => NotifyPropertyChanged(nameof(DeletedDomainVisibility));
    }
}
