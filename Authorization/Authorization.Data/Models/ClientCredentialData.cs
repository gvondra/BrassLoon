using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Models
{
    public class ClientCredentialData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid ClientCredentialId { get; set; }
        [ColumnMapping()] public Guid ClientId { get; set; }
        [ColumnMapping()] public byte[] Secret { get; set; }
        [ColumnMapping()] public bool IsActive { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
