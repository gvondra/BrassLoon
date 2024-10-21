using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogModels = BrassLoon.Interface.Log.Models;

namespace BrassLoon.Interface.Log
{
    public interface IExceptionService
    {
        Task<LogModels.Exception> Create(ISettings settings, LogModels.Exception exception);
#pragma warning disable S3427 // Method overloads with default parameter values should not overlap
        Task<LogModels.Exception> Create(
            ISettings settings,
            Guid domainId,
            Exception exception,
            DateTime? createTimestamp = null,
            string category = null,
            string level = null,
            LogModels.EventId? eventId = null);
#pragma warning restore S3427 // Method overloads with default parameter values should not overlap
        Task<LogModels.Exception> Create(ISettings settings, Guid domainId, Exception exception);
        [Obsolete("use overload")]
        Task<LogModels.Exception> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, Exception exception);
        Task<List<LogModels.Exception>> Search(ISettings settings, Guid domainId, DateTime maxTimestamp);
        Task<LogModels.Exception> Get(ISettings settings, Guid domainId, long id);
    }
}
