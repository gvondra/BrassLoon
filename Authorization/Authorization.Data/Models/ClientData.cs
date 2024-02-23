using BrassLoon.DataClient;
using System;

namespace BrassLoon.Authorization.Data.Models
{
    public class ClientData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid ClientId { get; set; }
        [ColumnMapping] public Guid DomainId { get; set; }
        [ColumnMapping] public string Name { get; set; }
        [ColumnMapping] public Guid SecretKey { get; set; }
        [ColumnMapping] public byte[] SecretSalt { get; set; }
        [ColumnMapping] public bool IsActive { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
        [ColumnMapping(IsOptional = true)] public Guid? UserEmailAddressId { get; set; }
        [ColumnMapping(IsOptional = true)] public string UserName { get; set; } = string.Empty;
    }
}
