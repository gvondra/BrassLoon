using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IExceptionFactory
    {
        IException Create(Guid domainId, DateTime? createTimestamp);
        IException Create(Guid domainId, DateTime? createTimestamp, IException parentException);
        Task<IException> Get(ISettings settings, long id);
        Task<IException> GetInnerException(ISettings settings, long id);
        Task<IEnumerable<IException>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, DateTime maxTimestamp);
    }
}
