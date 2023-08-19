using BrassLoon.DataClient;
using System;

namespace BrassLoon.Config.Data.Models
{
    public class LookupData : DataManagedStateBase
    {
        [ColumnMapping("LookupId", IsPrimaryKey = true)] public Guid LookupId { get; set; }
        [ColumnMapping("DomainId")] public Guid DomainId { get; set; }
        [ColumnMapping("Code")] public string Code { get; set; }
        [ColumnMapping("Data")] public string Data { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
