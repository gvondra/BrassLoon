using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IException
    {
        long ExceptionId { get; }
        Guid DomainId { get; }
        string Message { get; set; }
        string TypeName { get; set; }
        string Source { get; set; }
        string AppDomain { get; set; }
        string TargetSite { get; set; }
        string StackTrace { get; set; }
        dynamic Data { get; set; }
        DateTime CreateTimestamp { get; }

        Task<IException> GetInnerException(ISettings settings);
        Task Create(ITransactionHandler transactionHandler);
    }
}
