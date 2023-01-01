using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IEventId
    {
        Guid EventId { get; }
        Guid DomainId { get; }
        int Id { get; }
        string Name { get; }
        DateTime CreateTimestamp { get; }

        Task Create(ITransactionHandler transactionHandler);
    }
}
