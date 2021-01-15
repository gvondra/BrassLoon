using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Config.Data.Models
{
    public class LookupHistoryData
    {
        [ColumnMapping("LookupHistoryId", IsPrimaryKey = true)] public Guid LookupHistoryId { get; set; }
        [ColumnMapping("LookupId")] public Guid LookupId { get; set; }
        [ColumnMapping("DomainId")] public Guid DomainId { get; set; }
        [ColumnMapping("Code")] public string Code { get; set; }
        [ColumnMapping("Data")] public string Data { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
