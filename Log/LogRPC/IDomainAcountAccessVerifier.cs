using System;
using System.Threading.Tasks;

namespace LogRPC
{
    public interface IDomainAcountAccessVerifier
    {
        Task<bool> HasAccess(Settings settings, Guid domainId, string accessToken);
    }
}
