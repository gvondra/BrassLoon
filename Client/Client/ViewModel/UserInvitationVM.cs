using BrassLoon.Interface.Account.Models;

namespace BrassLoon.Client.ViewModel
{
    public class UserInvitationVM : ViewModelBase
    {
        private readonly UserInvitation _userInvitation;

        public UserInvitationVM(UserInvitation userInvitation)
        {
            _userInvitation = userInvitation;
        }

        public string EmailAddress => _userInvitation.EmailAddress;
    }
}
