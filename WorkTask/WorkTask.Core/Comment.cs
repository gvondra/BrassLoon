using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public abstract class Comment : IComment
    {
        private readonly CommentData _data;

        protected Comment(CommentData data)
        {
            _data = data;
        }

        public Guid CommentId => _data.CommentId;

        public Guid DomainId => _data.DomainId;

        public string Text => _data.Text;

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public abstract Task Create(ISaveSettings settings);
    }
}
