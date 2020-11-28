using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface ITrace
    {
        long TraceId { get; }
        Guid DomainId { get; }
        string EventCode { get; }
        string Message { get; set; }
        dynamic Data { get; set; }
        DateTime CreateTimestamp { get; }

        Task Create(ITransactionHandler transactionHandler);
    }
}
