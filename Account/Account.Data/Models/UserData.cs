using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Data.Models
{
    public class UserData : DataManagedStateBase
    {
        [ColumnMapping("UserGuid", IsPrimaryKey = true)] public Guid UserGuid { get; set; }
        [ColumnMapping("ReferenceId")] public string ReferenceId { get; set; }
        [ColumnMapping("Name")] public string Name { get; set; }
        [ColumnMapping("EmailAddressGuid")] public Guid EmailAddressGuid { get; set; }
        [ColumnMapping("Roles")] public short Roles { get; set; }
        [ColumnMapping("CreateTimestamp")] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp")] public DateTime UpdateTimestamp { get; set; }
    }
}
