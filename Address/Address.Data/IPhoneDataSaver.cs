using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IPhoneDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, PhoneData data);
    }
}
