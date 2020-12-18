using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IDomainDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, DomainData domainData);
        Task Update(ISqlTransactionHandler transactionHandler, DomainData domainData);
    }
}
