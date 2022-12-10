using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Models
{
    public class EmailAddressData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid EmailAddressId { get; set; }
        [ColumnMapping()] public string Address { get; set; }
        [ColumnMapping()] public byte[] AddressHash { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
