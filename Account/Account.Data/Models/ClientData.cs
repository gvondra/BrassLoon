using BrassLoon.DataClient;
using System;

namespace BrassLoon.Account.Data.Models
{
    public class ClientData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid ClientId { get; set; }
        [ColumnMapping("AccountGuid")] public Guid AccountId { get; set; }
        [ColumnMapping] public string Name { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
        [ColumnMapping] public short SecretType { get; set; }
        [ColumnMapping] public Guid? SecretKey { get; set; }
        [ColumnMapping] public byte[] SecretSalt { get; set; }
        [ColumnMapping] public bool IsActive { get; set; } = true;
    }
}
