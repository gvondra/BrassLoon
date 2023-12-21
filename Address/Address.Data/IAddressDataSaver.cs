using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IAddressDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, AddressData data);
    }
}
