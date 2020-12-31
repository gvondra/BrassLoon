using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Data.Models
{
    public class ClientCredentialData : DataManagedStateBase
    {
        [ColumnMapping("ClientCredentialId", IsPrimaryKey = true)] public Guid ClientCredentialId { get; set; }
        [ColumnMapping("ClientId")] public Guid ClientId { get; set; }
        [ColumnMapping("Secret")] public byte[] Secret { get; set; }
        [ColumnMapping("IsActive")] public bool IsActive { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
