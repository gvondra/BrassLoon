using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Config.Data.Models
{
    public class LookupData : DataManagedStateBase
    {
        [ColumnMapping("LookupId", IsPrimaryKey = true)]
        [BsonId]
        public Guid LookupId { get; set; }

        [ColumnMapping("DomainId")]
        public Guid DomainId { get; set; }

        [ColumnMapping("Code")]
        [BsonRequired]
        public string Code { get; set; }

        [ColumnMapping("Data")]
        [BsonRequired]
        public string Data { get; set; }

        [ColumnMapping("CreateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping("UpdateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }
    }
}
