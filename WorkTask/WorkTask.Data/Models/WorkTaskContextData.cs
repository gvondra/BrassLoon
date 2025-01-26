using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkTaskContextData : DataManagedStateBase
    {
        [ColumnMapping(IsUtc = true)]
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkTaskContextId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkTaskId { get; set; }

        [ColumnMapping]
        public short Status { get; set; }

        [ColumnMapping]
        public short ReferenceType { get; set; }

        [ColumnMapping]
        public string ReferenceValue { get; set; }

        [ColumnMapping]
        public byte[] ReferenceValueHash { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }
    }
}
