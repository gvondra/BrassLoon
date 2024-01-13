using BrassLoon.DataClient;
using System;

namespace BrassLoon.Account.Data.Models
{
    public class UserInvitationData : DataManagedStateBase
    {
        [ColumnMapping("UserInvitationId", IsPrimaryKey = true)] public Guid UserInvitationId { get; set; }
        [ColumnMapping("AccountGuid")] public Guid AccountId { get; set; }
        [ColumnMapping("EmailAddressGuid")] public Guid EmailAddressId { get; set; }
        [ColumnMapping("Status")] public short Status { get; set; }
        [ColumnMapping("ExpirationTimestamp", IsUtc = true)] public DateTime ExpirationTimestamp { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
