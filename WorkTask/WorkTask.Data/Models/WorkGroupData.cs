using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkGroupData : DataManagedStateBase
    {
        private List<WorkGroupMemberData> _members = new List<WorkGroupMemberData>();

        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkGroupId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        [BsonRequired]
        public string Title { get; set; }

        [ColumnMapping]
        public string Description { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }

        public List<WorkGroupMemberData> Members
        {
            get => _members ?? new List<WorkGroupMemberData>();
            set => _members = value ?? new List<WorkGroupMemberData>();
        }

        [BsonIgnore]
        public List<WorkTaskTypeGroupData> TaskTypes { get; set; }
    }
}
