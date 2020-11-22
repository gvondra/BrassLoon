using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IExceptionDataSaver
    {
        Task Create(ITransactionHandler transactionHandler, ExceptionData exceptionData);
    }
}
