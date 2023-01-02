using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogModels = BrassLoon.Interface.Log.Models;

namespace BrassLoon.Interface.Log
{
    public interface IExceptionService
    {
        Task<LogModels.Exception> Create(ISettings settings, LogModels.Exception exception);
        Task<LogModels.Exception> Create(ISettings settings, 
            Guid domainId, 
            System.Exception exception, 
            DateTime? createTimestamp = null, 
            string category = null, 
            string level = null,
            LogModels.EventId? eventId = null);
        Task<LogModels.Exception> Create(ISettings settings, Guid domainId, System.Exception exception);
        [Obsolete()]
        Task<LogModels.Exception> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, System.Exception exception);
        Task<List<LogModels.Exception>> Search(ISettings settings, Guid domainId, DateTime maxTimestamp);
        Task<LogModels.Exception> Get(ISettings settings, Guid domainId, long id);
    }
}
