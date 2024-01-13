using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IUserInvitationDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, UserInvitationData userInvitationData);
        Task Update(ISqlTransactionHandler transactionHandler, UserInvitationData userInvitationData);
    }
}
