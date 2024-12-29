using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Log.Data.Models
{
    public class EventIdData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid EventId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        public int Id { get; set; }

        [ColumnMapping]
        [BsonRequired]
        public string Name { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }
    }
}
