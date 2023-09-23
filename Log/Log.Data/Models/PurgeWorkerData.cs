using BrassLoon.DataClient;
using System;

namespace BrassLoon.Log.Data.Models
{
    public class PurgeWorkerData : DataManagedStateBase
    {
        [ColumnMapping("PurgeWorkerId", IsPrimaryKey = true)] public Guid PurgeWorkerId { get; set; }
        [ColumnMapping("DomainId")] public Guid DomainId { get; set; }
        [ColumnMapping("Status")] public short Status { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
