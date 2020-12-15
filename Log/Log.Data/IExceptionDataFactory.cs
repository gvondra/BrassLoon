using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IExceptionDataFactory
    {
        Task<ExceptionData> Get(ISettings settings, long id);
        Task<ExceptionData> GetInnerException(ISettings settings, long id);
        Task<IEnumerable<ExceptionData>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, DateTime maxTimestamp);
    }
}
