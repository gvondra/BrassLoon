using BrassLoon.DataClient;
using System;

namespace BrassLoon.Log.Data.Models
{
    public class TraceData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public long TraceId { get; set; }
        [ColumnMapping()] public Guid DomainId { get; set; }
        [ColumnMapping()] public string EventCode { get; set; }
        [ColumnMapping()] public string Message { get; set; }
        [ColumnMapping()] public string Data { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping()] public Guid? EventId { get; set; }
        [ColumnMapping()] public string Category { get; set; }
        [ColumnMapping()] public string Level { get; set; }
    }
}
