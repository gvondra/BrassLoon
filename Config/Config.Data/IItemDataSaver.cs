using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface IItemDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, ItemData itemData);
        Task Update(ISqlTransactionHandler transactionHandler, ItemData itemData);
        Task DeleteByCode(ISqlTransactionHandler transactionHandler, Guid domainId, string code);
    }
}
