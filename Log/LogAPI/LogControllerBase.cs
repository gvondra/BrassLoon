using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogAPI
{
    public abstract class LogControllerBase : ControllerBase
    {
        private static Policy m_cache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromSeconds(90)));

        [NonAction]
        protected async Task<bool> VerifyDomainAccount(Guid domainId, SettingsFactory settingsFactory, Settings settings, IDomainService domainService)
        {
            Domain domain = await GetDomain(
                domainId,
                settingsFactory,
                settings,
                GetAccessToken(),
                domainService
                );
            return VerifyDomainAccount(domain);
        }

        private async Task<Domain> GetDomain(Guid domainId, SettingsFactory settingsFactory, Settings settings, string accessToken, IDomainService domainService)
        {
            HashAlgorithm hashAlgorithm = SHA256.Create();
            string hash = Convert.ToBase64String(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(accessToken)));
            return await m_cache.Execute<Task<Domain>>(async context =>
            {
                return await domainService.Get(
                settingsFactory.CreateAccount(settings, accessToken),
                domainId
                );
            },
            new Context(string.Format("{0:N}::{1}::{2}", domainId, hash, settings.AccountApiBaseAddress))
            );
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
