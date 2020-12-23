using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Data.Models
{
    public class PurgeData : DataManagedStateBase
    {
        [ColumnMapping("PurgeId", IsPrimaryKey = true)] public long PurgeId { get; set; }
        [ColumnMapping("DomainId")] public Guid DomainId { get; set; }
        [ColumnMapping("Status")] public short Status { get; set; }
        [ColumnMapping("TargetId")] public long TargetId { get; set; }
        [ColumnMapping("ExpirationTimestamp", IsUtc = true)] public DateTime ExpirationTimestamp { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
