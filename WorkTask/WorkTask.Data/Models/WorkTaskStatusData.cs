using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkTaskStatusData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkTaskStatusId { get; set; }

        [ColumnMapping]
        [BsonIgnore]
        public Guid WorkTaskTypeId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        [BsonRequired]
        public string Code { get; set; }

        [ColumnMapping]
        public string Name { get; set; }

        [ColumnMapping]
        public string Description { get; set; }

        [ColumnMapping]
        public bool IsDefaultStatus { get; set; }

        [ColumnMapping]
        public bool IsClosedStatus { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }

        [ColumnMapping]
        public int WorkTaskCount { get; set; }
    }
}
