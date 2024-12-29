using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Log.Data.Models
{
    public class PurgeWorkerData : DataManagedStateBase
    {
        [ColumnMapping("PurgeWorkerId", IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid PurgeWorkerId { get; set; }

        [ColumnMapping("DomainId")]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping("Status")]
        public short Status { get; set; }

        [ColumnMapping("CreateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping("UpdateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }
    }
}
