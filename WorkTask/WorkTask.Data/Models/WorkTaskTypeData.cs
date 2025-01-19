using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkTaskTypeData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkTaskTypeId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        [BsonRequired]
        public string Code { get; set; }

        [ColumnMapping]
        public string Title { get; set; }

        [ColumnMapping]
        public string Description { get; set; }

        [ColumnMapping]
        [BsonIgnoreIfNull]
        public short? PurgePeriod { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }

        [ColumnMapping]
        public int WorkTaskCount { get; set; }

        [BsonRequired]
        public List<WorkTaskStatusData> Statuses { get; set; } = new List<WorkTaskStatusData>();
    }
}
