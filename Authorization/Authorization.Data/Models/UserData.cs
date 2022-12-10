using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Models
{
    public class UserData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid UserId { get; set; }
        [ColumnMapping()] public Guid DomainId { get; set; }
        [ColumnMapping()] public string ReferenceId { get; set; }
        [ColumnMapping()] public Guid EmailAddressId { get; set; }
        [ColumnMapping()] public string Name { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
