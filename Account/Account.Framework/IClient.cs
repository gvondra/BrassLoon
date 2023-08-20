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

        void SetSecret(string secret);
        /// <returns>Returns true if the client is active and the given secret matches the stored secret.</returns>
        Task<bool> AuthenticateSecret(ISettings settings, string secret);
        Task Create(ITransactionHandler transactionHandler, ISettings settings);
        Task Update(ITransactionHandler transactionHandler, ISettings settings);
        Task<byte[]> GetSecretHash(BrassLoon.CommonCore.ISettings settings);
    }
}
