using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Data.Models
{
    public class MetricData : DataManagedStateBase
    {
        [ColumnMapping("MetricId", IsPrimaryKey = true)] public long MetricId { get; set; }
        [ColumnMapping("DomainId")] public Guid DomainId { get; set; }
        [ColumnMapping("EventCode")] public string EventCode { get; set; }
        [ColumnMapping("Magnitude")] public double? Magnitude { get; set; }
        [ColumnMapping("Data")] public string Data { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("Status")] public string Status { get; set; }
        [ColumnMapping("Requestor")] public string Requestor { get; set; }
    }
}
