using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IClientDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, ClientData clientData);
        Task Update(ISqlTransactionHandler transactionHandler, ClientData clientData);
    }
}
