using System;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class Comment
    {
        public Guid? CommentId { get; set; }
        public Guid? DomainId { get; set; }
        public string Text { get; set; }
        public DateTime? CreateTimestamp { get; set; }

        internal static Comment Create(Protos.Comment comment)
        {
            return new Comment
            {
                CommentId = !string.IsNullOrEmpty(comment.CommentId) ? Guid.Parse(comment.CommentId) : default,
                CreateTimestamp = comment.CreateTimestamp != null ? comment.CreateTimestamp.ToDateTime() : default,
                DomainId = !string.IsNullOrEmpty(comment.DomainId) ? Guid.Parse(comment.DomainId) : default,
                Text = comment.Text
            };
        }
    }
}
