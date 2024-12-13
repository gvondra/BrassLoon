using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Account.Data.Models
{
    public class ClientCredentialData : DataManagedStateBase
    {
        [ColumnMapping("ClientCredentialId", IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid ClientCredentialId { get; set; }

        [ColumnMapping("ClientId")]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid ClientId { get; set; }

        [ColumnMapping("Secret")]
        public byte[] Secret { get; set; }

        [ColumnMapping("IsActive")]
        public bool IsActive { get; set; }

        [ColumnMapping("CreateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping("UpdateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }
    }
}
