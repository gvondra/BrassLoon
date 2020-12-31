using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Data.Models
{
    public class DomainData : DataManagedStateBase
    {
        [ColumnMapping("DomainGuid", IsPrimaryKey = true)] public Guid DomainGuid { get; set; }
        [ColumnMapping("AccountGuid")] public Guid AccountGuid { get; set; }
        [ColumnMapping("Name")] public string Name { get; set; }
        [ColumnMapping("Deleted")] public bool Deleted { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
