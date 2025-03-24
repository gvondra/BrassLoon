using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkTaskData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkTaskId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkTaskTypeId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkTaskStatusId { get; set; }

        [ColumnMapping]
        public string Title { get; set; }

        [ColumnMapping]
        public string Text { get; set; }

        [ColumnMapping]
        [BsonDefaultValue("")]
        public string AssignedToUserId { get; set; }

        [ColumnMapping]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified, DateOnly = true)]
        public DateTime? AssignedDate { get; set; }

        [ColumnMapping]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified, DateOnly = true)]
        public DateTime? ClosedDate { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }

        [BsonIgnore]
        public WorkTaskTypeData WorkTaskType { get; set; }

        [BsonIgnore]
        public WorkTaskStatusData WorkTaskStatus { get; set; }

        [BsonIgnoreIfNull]
        public List<WorkTaskContextData> WorkTaskContexts { get; set; }
    }
}
