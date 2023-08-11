using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogRPC
{
    public class DomainAcountAccessVerifier : IDomainAcountAccessVerifier
    {   
        private readonly SettingsFactory _settingsFactory;
        private readonly IDomainService _domainService;

        public DomainAcountAccessVerifier(
            SettingsFactory settingsFactory,
            IDomainService domainService)
        {
            _settingsFactory = settingsFactory;
            _domainService = domainService;
        }

        public async Task<bool> HasAccess(Settings settings, Guid domainId, string accessToken)
        {
            AccountDomain accountDomain = await GetDomain(domainId, settings, accessToken, _domainService);
            return accountDomain != null && !accountDomain.Account.Locked && VerifyDomainAccount(accountDomain, accessToken);
        }

        private bool VerifyDomainAccount(Domain domain, string accessToken)
        {
            return domain != null && UserCanAccessAccount(domain.AccountId.Value, accessToken);
        }

        private bool UserCanAccessAccount(Guid accountId, string accessToken)
        {
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(accessToken);            
            string[] accountIds = Regex.Split(jwtSecurityToken.Claims.First(c => c.Type == "accounts").Value, @"\s+", RegexOptions.IgnoreCase);
            return accountIds.Where(id => !string.IsNullOrEmpty(id)).Any(id => Guid.Parse(id).Equals(accountId));
        }

        private async Task<AccountDomain> GetDomain(Guid domainId, Settings settings, string accessToken, IDomainService domainService)
        {
            try
            {
                return await domainService.GetAccountDomain(
                    _settingsFactory.CreateAccount(settings, accessToken),
                    domainId);
            }
            catch (BrassLoon.RestClient.Exceptions.RequestError ex)
            {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    return null;
                else
                    throw;
            }
        }
    }
}
