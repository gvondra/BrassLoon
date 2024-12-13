using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Account.Data.Models
{
    public class ClientData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid ClientId { get; set; }

        [ColumnMapping("AccountGuid")]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid AccountId { get; set; }

        [ColumnMapping]
        [BsonRequired]
        public string Name { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }

        [ColumnMapping]
        public short SecretType { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid? SecretKey { get; set; }

        [ColumnMapping]
        public byte[] SecretSalt { get; set; }

        [ColumnMapping]
        public bool IsActive { get; set; } = true;
    }
}
