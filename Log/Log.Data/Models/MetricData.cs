using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Log.Data.Models
{
    public class MetricData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonIgnore]
        public long MetricId { get; set; }

        [ColumnMapping]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid MetricGuid { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        public string EventCode { get; set; }

        [ColumnMapping]
        public double? Magnitude { get; set; }

        [ColumnMapping]
        public string Data { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping]
        public string Status { get; set; }

        [ColumnMapping]
        public string Requestor { get; set; }

        [ColumnMapping]
        public Guid? EventId { get; set; }

        [ColumnMapping]
        public string Category { get; set; }

        [ColumnMapping]
        public string Level { get; set; }
    }
}
