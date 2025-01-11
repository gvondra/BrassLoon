using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface ISigningKeyDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, SigningKeyData data);
        Task Update(ISqlTransactionHandler transactionHandler, SigningKeyData data);
    }
}
