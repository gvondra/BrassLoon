using BrassLoon.CommonData;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IExceptionDataFactory
    {
        Task<ExceptionData> Get(ISettings settings, Guid id);
        Task<ExceptionData> GetInnerException(ISettings settings, ExceptionData data);
        Task<IEnumerable<ExceptionData>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, DateTime maxTimestamp);
    }
}
