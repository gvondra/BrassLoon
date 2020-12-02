using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogAPI
{
    public abstract class LogControllerBase : ControllerBase
    {
        [NonAction]
        protected async Task<bool> VerifyDomainAccount(Guid domainId, SettingsFactory settingsFactory, Settings settings, IDomainService domainService)
        {
            Domain domain = await domainService.Get(
                settingsFactory.CreateAccount(settings, GetAccessToken()),
                domainId
                );
            return VerifyDomainAccount(domain);
        }

        [NonAction]
        protected bool VerifyDomainAccount(Domain domain)
        {
            bool result = false;
            if (domain != null)
            {
                result = UserCanAccessAccount(domain.AccountId.Value);
            }            
            return result;
        }

        [NonAction]
        protected bool UserCanAccessAccount(Guid accountId)
        {
            string[] accountIds = Regex.Split(User.Claims.First(c => c.Type == "accounts").Value, @"\s+", RegexOptions.IgnoreCase);
            return accountIds.Where(id => !string.IsNullOrEmpty(id)).Any(id => Guid.Parse(id).Equals(accountId));
        }

        [NonAction]
        protected string GetAccessToken()
        {
            string token = this.Request.Headers.Where(h => string.Equals("Authorization", h.Key, StringComparison.OrdinalIgnoreCase))
                .Select(h => h.Value.FirstOrDefault())
                .FirstOrDefault();
            token = (token ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(token))
            {
                Match match = Regex.Match(token, @"(?<=^Bearer\s+).+$", RegexOptions.IgnoreCase);
                if (match.Success)
                    token = match.Value;
            }
            return token;
        }
    }
}
