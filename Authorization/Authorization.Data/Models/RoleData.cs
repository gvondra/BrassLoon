using BrassLoon.DataClient;
using System;

namespace BrassLoon.Authorization.Data.Models
{
    public class RoleData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid RoleId { get; set; }
        [ColumnMapping] public Guid DomainId { get; set; }
        [ColumnMapping] public string Name { get; set; }
        [ColumnMapping] public string PolicyName { get; set; }
        [ColumnMapping] public bool IsActive { get; set; } = true;
        [ColumnMapping] public string Comment { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
