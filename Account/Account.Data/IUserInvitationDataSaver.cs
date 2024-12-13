using BrassLoon.Account.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IUserInvitationDataSaver
    {
        Task Create(ISaveSettings settings, UserInvitationData userInvitationData);
        Task Update(ISaveSettings settings, UserInvitationData userInvitationData);
    }
}
