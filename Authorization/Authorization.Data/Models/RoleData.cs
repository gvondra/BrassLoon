using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Authorization.Data.Models
{
    public class RoleData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid RoleId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        public string Name { get; set; }

        [ColumnMapping]
        [BsonRequired]
        public string PolicyName { get; set; }

        [ColumnMapping]
        public bool IsActive { get; set; } = true;

        [ColumnMapping]
        public string Comment { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }
    }
}
