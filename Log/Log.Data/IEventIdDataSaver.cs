using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IEventIdDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, EventIdData data);
    }
}
