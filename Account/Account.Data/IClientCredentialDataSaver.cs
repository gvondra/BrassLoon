using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IClientCredentialDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, ClientCredentialData clientCredentialData);
    }
}
