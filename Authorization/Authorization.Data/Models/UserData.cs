using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace BrassLoon.Authorization.Data.Models
{
    public class UserData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid UserId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        public string ReferenceId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid EmailAddressId { get; set; }

        [ColumnMapping]
        public string Name { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }

        [BsonIgnoreIfNull]
        public List<Guid> RoleIds { get; set; }
    }
}
