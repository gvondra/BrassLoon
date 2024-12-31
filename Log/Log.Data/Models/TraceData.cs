using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Log.Data.Models
{
    public class TraceData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonIgnore]
        public long TraceId { get; set; }

        [ColumnMapping]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid TraceGuid { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }
        [ColumnMapping]
        public string EventCode { get; set; }

        [ColumnMapping]
        public string Message { get; set; }

        [ColumnMapping]
        public string Data { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid? EventId { get; set; }

        [ColumnMapping]
        public string Category { get; set; }

        [ColumnMapping]
        public string Level { get; set; }
    }
}
