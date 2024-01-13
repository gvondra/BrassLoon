using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IUserDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, UserData userData);
        Task Update(ISqlTransactionHandler transactionHandler, UserData userData);
    }
}
