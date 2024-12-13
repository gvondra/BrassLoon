using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Account.Data.Models
{
    public class UserData : DataManagedStateBase
    {
        [ColumnMapping("UserGuid", IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid UserGuid { get; set; }

        [ColumnMapping("ReferenceId")]
        [BsonRequired]
        public string ReferenceId { get; set; }

        [ColumnMapping("Name")]
        [BsonRequired]
        public string Name { get; set; }

        [ColumnMapping("EmailAddressGuid")]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid EmailAddressGuid { get; set; }

        [ColumnMapping("Roles")]
        public short Roles { get; set; }

        [ColumnMapping("CreateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping("UpdateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }
    }
}
