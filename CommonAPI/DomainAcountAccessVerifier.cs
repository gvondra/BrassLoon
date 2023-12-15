using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.CommonAPI
{
    public class DomainAcountAccessVerifier : IDomainAcountAccessVerifier
    {
        private readonly IDomainService _domainService;

        public DomainAcountAccessVerifier(IDomainService domainService)
        {
            _domainService = domainService;
        }

        public async Task<bool> HasAccess(ISettings settings, Guid domainId, string accessToken)
        {
            AccountDomain accountDomain = await GetDomain(_domainService, domainId, settings);
            return accountDomain != null && !accountDomain.Account.Locked && VerifyDomainAccount(accountDomain, accessToken);
        }

        private static bool VerifyDomainAccount(Domain domain, string accessToken) => domain != null && UserCanAccessAccount(domain.AccountId.Value, accessToken);

        private static bool UserCanAccessAccount(Guid accountId, string accessToken)
        {
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(accessToken);
            string[] accountIds = Regex.Split(jwtSecurityToken.Claims.First(c => c.Type == "accounts").Value, @"\s+", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200));
            return accountIds.Where(id => !string.IsNullOrEmpty(id)).Any(id => Guid.Parse(id).Equals(accountId));
        }

        private static async Task<AccountDomain> GetDomain(IDomainService domainService, Guid domainId, ISettings settings)
        {
            try
            {
                return await domainService.GetAccountDomain(settings, domainId);
            }
            catch (RestClient.Exceptions.RequestError ex)
            {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    return null;
                else
                    throw;
            }
        }
    }
}
