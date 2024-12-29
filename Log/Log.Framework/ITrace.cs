using BrassLoon.CommonCore;
using System;
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
        string Category { get; set; }
        string Level { get; set; }

        Task Create(ISaveSettings settings);
    }
}
