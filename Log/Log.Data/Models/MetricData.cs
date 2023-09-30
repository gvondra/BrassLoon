using BrassLoon.DataClient;
using System;

namespace BrassLoon.Log.Data.Models
{
    public class MetricData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public long MetricId { get; set; }
        [ColumnMapping()] public Guid DomainId { get; set; }
        [ColumnMapping()] public string EventCode { get; set; }
        [ColumnMapping()] public double? Magnitude { get; set; }
        [ColumnMapping()] public string Data { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping()] public string Status { get; set; }
        [ColumnMapping()] public string Requestor { get; set; }
        [ColumnMapping()] public Guid? EventId { get; set; }
        [ColumnMapping()] public string Category { get; set; }
        [ColumnMapping()] public string Level { get; set; }
    }
}
