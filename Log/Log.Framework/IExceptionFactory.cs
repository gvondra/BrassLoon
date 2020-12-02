using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IExceptionFactory
    {
        IException Create(Guid domainId);
        IException Create(Guid domainId, IException parentException);
        Task<IException> GetInnerException(ISettings settings, long id);
    }
}
