using BrassLoon.Account.Framework.Enumerations;
using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IClient
    {
        Guid ClientId { get; }
        Guid AccountId { get; }
        string Name { get; set; }
        bool IsActive { get; set; }
        SecretType SecretType { get; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);
        Task<byte[]> GetSecretHash(ISettings settings);
    }
}
