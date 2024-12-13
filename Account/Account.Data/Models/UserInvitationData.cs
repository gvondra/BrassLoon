using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Account.Data.Models
{
    public class UserInvitationData : DataManagedStateBase
    {
        [ColumnMapping("UserInvitationId", IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid UserInvitationId { get; set; }

        [ColumnMapping("AccountGuid")]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid AccountId { get; set; }

        [ColumnMapping("EmailAddressGuid")]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid EmailAddressId { get; set; }

        [ColumnMapping("Status")]
        public short Status { get; set; }

        [ColumnMapping("ExpirationTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ExpirationTimestamp { get; set; }

        [ColumnMapping("CreateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping("UpdateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }
    }
}
