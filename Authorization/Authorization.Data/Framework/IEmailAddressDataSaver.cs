using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Framework
{
    public interface IEmailAddressDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, EmailAddressData data);
    }
}
