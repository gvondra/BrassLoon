using BrassLoon.Account.Framework.Enumerations;
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

        void SetSecret(string secret, SecretType secretType);
        /// <returns>Returns true if the client is active and the given secret matches the stored secret.</returns>
        Task<bool> AuthenticateSecret(ISettings settings, string secret);
        Task Create(ISaveSettings settings);
        Task Update(ISaveSettings settings);
        Task<byte[]> GetSecretHash(ISettings settings);
    }
}
