using BrassLoon.Interface.Account.Models;
using System;
using System.Windows.Input;

namespace BrassLoon.Client.ViewModel
{
    public class UserInvitationVM : ViewModelBase
    {
        private readonly UserInvitation _userInvitation;
        private ICommand _cancel;

        public UserInvitationVM(UserInvitation userInvitation)
        {
            _userInvitation = userInvitation;
        }

        public UserInvitation InnerInvitation => _userInvitation;
        public string EmailAddress => _userInvitation.EmailAddress;
        public DateTime? EpirationTimestamp => _userInvitation.ExpirationTimestamp.HasValue ? _userInvitation.ExpirationTimestamp.Value.ToLocalTime() : default;
        public DateTime? CreateTimestamp => _userInvitation.CreateTimestamp.HasValue ? _userInvitation.CreateTimestamp.Value.ToLocalTime() : default;

        public ICommand Cancel
        {
            get => _cancel;
            set
            {
                if (_cancel != value)
                {
                    _cancel = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
