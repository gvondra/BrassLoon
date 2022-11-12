using LogModels = BrassLoon.Interface.Log.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public interface IExceptionService
    {
        Task<LogModels.Exception> Create(ISettings settings, LogModels.Exception exception);
        Task<LogModels.Exception> Create(ISettings settings, Guid domainId, System.Exception exception);
        Task<LogModels.Exception> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, System.Exception exception);
        Task<List<LogModels.Exception>> Search(ISettings settings, Guid domainId, DateTime maxTimestamp);
        Task<LogModels.Exception> Get(ISettings settings, Guid domainId, long id);
    }
}
