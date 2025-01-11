using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Authorization.Data.Models
{
    public class ClientData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid ClientId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        public string Name { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid SecretKey { get; set; }

        [ColumnMapping]
        public byte[] SecretSalt { get; set; }

        [ColumnMapping]
        public bool IsActive { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }

        [ColumnMapping(IsOptional = true)]
        [BsonIgnoreIfNull]
        public Guid? UserEmailAddressId { get; set; }

        [ColumnMapping(IsOptional = true)]
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        [BsonDefaultValue("")]
        public string UserName { get; set; } = string.Empty;
    }
}
