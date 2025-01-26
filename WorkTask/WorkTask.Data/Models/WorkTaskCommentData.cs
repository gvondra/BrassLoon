using MongoDB.Bson.Serialization.Attributes;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkTaskCommentData
    {
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        public Guid WorkTaskId { get; set; }
        public CommentData Comment { get; set; }
    }
}
