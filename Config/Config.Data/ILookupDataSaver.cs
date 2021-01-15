using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface ILookupDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, LookupData lookupData);
        Task Update(ISqlTransactionHandler transactionHandler, LookupData lookupData);
        Task DeleteByCode(ISqlTransactionHandler transactionHandler, Guid domainId, string code);
    }
}
