using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Log.Data.Models
{
    public class ExceptionData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        public long ExceptionId { get; set; }

        [ColumnMapping]
        public long? ParentExceptionId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        public string Message { get; set; }

        [ColumnMapping]
        public string TypeName { get; set; }

        [ColumnMapping]
        public string Source { get; set; }

        [ColumnMapping]
        public string AppDomain { get; set; }

        [ColumnMapping]
        public string TargetSite { get; set; }

        [ColumnMapping]
        public string StackTrace { get; set; }

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
