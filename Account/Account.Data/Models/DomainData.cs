using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Account.Data.Models
{
    public class DomainData : DataManagedStateBase
    {
        [ColumnMapping("DomainGuid", IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid DomainGuid { get; set; }

        [ColumnMapping("AccountGuid")]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid AccountGuid { get; set; }

        [ColumnMapping("Name")]
        [BsonRequired]
        public string Name { get; set; }

        [ColumnMapping("Deleted")]
        public bool Deleted { get; set; }

        [ColumnMapping("CreateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping("UpdateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }
    }
}
