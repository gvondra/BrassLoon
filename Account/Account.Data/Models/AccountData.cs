using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Data.Models
{
    public class AccountData : DataManagedStateBase
    {
        [ColumnMapping("AccountGuid", IsPrimaryKey = true)] public Guid AccountGuid { get; set; }
        [ColumnMapping("Name")] public string Name { get; set; }
        [ColumnMapping("CreateTimestamp")] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp")] public DateTime UpdateTimestamp { get; set; }
    }
}
