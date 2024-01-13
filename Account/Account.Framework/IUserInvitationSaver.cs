using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IUserInvitationSaver
    {
        Task Create(ISettings settings, IUserInvitation userInvitation);
        Task Update(ISettings settings, IUserInvitation userInvitation);
    }
}
