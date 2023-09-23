using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IExceptionDataFactory
    {
        Task<ExceptionData> Get(ISqlSettings settings, long id);
        Task<ExceptionData> GetInnerException(ISqlSettings settings, long id);
        Task<IEnumerable<ExceptionData>> GetTopBeforeTimestamp(ISqlSettings settings, Guid domainId, DateTime maxTimestamp);
    }
}
