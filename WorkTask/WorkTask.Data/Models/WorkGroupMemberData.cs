using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkGroupMemberData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkGroupMemberId { get; set; }

        [ColumnMapping]
        [BsonIgnore]
        public Guid WorkGroupId { get; set; }

        [ColumnMapping]
        [BsonIgnore]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        [BsonRequired]
        public string UserId { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }
    }
}
