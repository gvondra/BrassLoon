using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkTaskTypeGroupData : DataManagedStateBase
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [ColumnMapping(IsPrimaryKey = true)]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkTaskTypeId { get; set; }

        [ColumnMapping(IsPrimaryKey = true)]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkGroupId { get; set; }
    }
}
