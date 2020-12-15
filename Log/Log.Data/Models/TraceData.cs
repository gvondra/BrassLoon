using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Data.Models
{
    public class TraceData : DataManagedStateBase
    {
        [ColumnMapping("TraceId", IsPrimaryKey = true)] public long TraceId { get; set; }
        [ColumnMapping("DomainId")] public Guid DomainId { get; set; }
        [ColumnMapping("EventCode")] public string EventCode { get; set; }
        [ColumnMapping("Message")] public string Message { get; set; }
        [ColumnMapping("Data")] public string Data { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
