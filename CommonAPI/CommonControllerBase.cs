﻿using BrassLoon.Interface.Account;
using BrassLoon.Interface.Account.Models;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.CommonAPI
{
    public abstract class CommonControllerBase : ControllerBase
    {
        private static readonly Policy _cache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromSeconds(90)));

        [NonAction]
        protected string GetAccessToken()
        {
            string token = Request.Headers.Where(h => string.Equals("Authorization", h.Key, StringComparison.OrdinalIgnoreCase))
                .Select(h => h.Value.FirstOrDefault())
                .FirstOrDefault();
            token = (token ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(token))
            {
                Match match = Regex.Match(token, @"(?<=^Bearer\s+).+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200));
                if (match.Success)
                    token = match.Value;
            }
            return token;
        }

        [NonAction]
        protected virtual bool UserCanAccessAccount(Guid accountId)
        {
            string[] accountIds = Regex.Split(User.Claims.First(c => c.Type == "accounts").Value, @"\s+", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200));
            return accountIds.Where(id => !string.IsNullOrEmpty(id)).Any(id => Guid.Parse(id).Equals(accountId));
        }

        [NonAction]
        protected async Task<AccountDomain> GetDomain(Guid domainId, CommonApiSettings settings, string accessToken, IDomainService domainService)
        {
            try
            {
                string hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(accessToken)));
                return await _cache.Execute(
                    async context =>
                    {
                        return await domainService.GetAccountDomain(
                        CreateAccountSettings(settings, accessToken),
                        domainId);
                    },
                    new Context(string.Format(CultureInfo.InvariantCulture, "{0:N}::{1}::{2}", domainId, hash, settings.AccountApiBaseAddress)));
            }
            catch (RestClient.Exceptions.RequestError ex)
            {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    return null;
                else
                    throw;
            }
        }

        [NonAction]
        protected virtual async Task<bool> VerifyDomainAccountWriteAccess(Guid domainId, CommonApiSettings settings, IDomainService domainService)
        {
            AccountDomain domain = await GetDomain(
            domainId,
            settings,
            GetAccessToken(),
            domainService);
            return domain != null && !domain.Account.Locked && VerifyDomainAccount(domain);
        }

        [NonAction]
        protected async Task<bool> VerifyDomainAccount(Guid domainId, CommonApiSettings settings, IDomainService domainService)
        {
            Domain domain = await GetDomain(
                domainId,
                settings,
                GetAccessToken(),
                domainService);
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
        protected string GetCurrentUserReferenceId()
            => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        [NonAction]
        protected abstract Interface.Account.ISettings CreateAccountSettings(CommonApiSettings settings, string accessToken);
        [NonAction]
        protected abstract Interface.Log.ISettings CreateLogSettings(CommonApiSettings settings, string accessToken);
        [NonAction]
        protected virtual Interface.Log.ISettings CreateLogSettings(CommonApiSettings settings) => throw new NotImplementedException();

        [NonAction]
        protected async Task LogException(Exception ex, IExceptionService exceptionService, CommonApiSettings settings)
        {
            try
            {
                Console.WriteLine(ex.ToString());
                if (settings.LoggingDomainId.HasValue
                    && settings.LoggingClientId.HasValue
                    && !string.IsNullOrEmpty(settings.LoggingClientSecret)
                    && !string.IsNullOrEmpty(settings.LogApiBaseAddress))
                {
                    _ = await exceptionService.Create(
                        CreateLogSettings(settings),
                        settings.LoggingDomainId.Value,
                        ex);
                }
                else
                {
                    Guid loggingDomainId = Guid.Empty;
                    bool loggingDomainIdIsSet = false;
                    if (!string.IsNullOrEmpty(settings.ExceptionLoggingDomainId))
                        loggingDomainIdIsSet = Guid.TryParse(settings.ExceptionLoggingDomainId, out loggingDomainId);
                    if (loggingDomainIdIsSet && !string.IsNullOrEmpty(settings.LogApiBaseAddress))
                    {
                        _ = await exceptionService.Create(
                            CreateLogSettings(settings, GetAccessToken()),
                            loggingDomainId,
                            ex);
                    }
                }
            }
            catch (Exception innerException)
            {
                Console.WriteLine(innerException.ToString());
            }
        }
    }
}
