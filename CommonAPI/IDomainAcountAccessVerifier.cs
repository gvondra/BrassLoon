using BrassLoon.Interface.Account;
using System.Threading.Tasks;

namespace BrassLoon.CommonAPI
{
    public interface IDomainAcountAccessVerifier
    {
        Task<bool> HasAccess(ISettings settings, Guid domainId, string accessToken);
    }
}
