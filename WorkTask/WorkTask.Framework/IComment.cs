using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IComment
    {
        Guid CommentId { get; }
        Guid DomainId { get; }
        string Text { get; }
        DateTime CreateTimestamp { get; }

        Task Create(ITransactionHandler transactionHandler);
    }
}
