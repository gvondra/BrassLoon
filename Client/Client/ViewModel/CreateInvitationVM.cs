using BrassLoon.Interface.Account.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class CreateInvitationVM : ViewModelBase
    {
        private readonly AccountVM _account;
        private readonly UserInvitation _invitation;
        private Visibility _nextInstructionVisibility = Visibility.Collapsed;
        private ICommand _create;

        public CreateInvitationVM(AccountVM account)
        {
            _account = account;
            _invitation = new UserInvitation();
            ExpirationTimestamp = DateTime.Now.AddDays(7).Date;
        }

        public UserInvitation InnerInvitation => _invitation;

        public Guid AccountId => _account.AccountId;

        public ICommand Create
        {
            get => _create;
            set
            {
                if (_create != value)
                {
                    _create = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Visibility NextInstructionVisibility
        {
            get => _nextInstructionVisibility;
            set
            {
                if (_nextInstructionVisibility != value)
                {
                    _nextInstructionVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string EmailAddress
        {
            get => _invitation.EmailAddress;
            set
            {
                if (_invitation.EmailAddress != value)
                {
                    _invitation.EmailAddress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime? ExpirationTimestamp
        {
            get => _invitation.ExpirationTimestamp;
            set
            {
                if (_invitation.ExpirationTimestamp.HasValue != value.HasValue
                    || (value.HasValue && _invitation.ExpirationTimestamp.Value != value.Value))
                {
                    _invitation.ExpirationTimestamp = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
