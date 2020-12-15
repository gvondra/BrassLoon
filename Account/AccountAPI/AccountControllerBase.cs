using BrassLoon.Account.Framework;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AccountAPI
{
    public abstract class AccountControllerBase : ControllerBase
    {
        [NonAction]
        protected async Task<IUser> GetUser(IUserFactory userFactory, CoreSettings settings)
        {
            string referenceId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return await userFactory.GetByReferenceId(settings, referenceId);
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

        [NonAction]
        protected async Task LogException(Exception ex, IExceptionService exceptionService, SettingsFactory settingsFactory, Settings settings)
        {
            try
            {
                Console.WriteLine(ex.ToString());
                if (!string.IsNullOrEmpty(settings.ExceptionLoggingDomainId) && !string.IsNullOrEmpty(settings.LogApiBaseAddress))
                    await exceptionService.Create(
                        settingsFactory.CreateLog(settings, GetAccessToken()),
                        Guid.Parse(settings.ExceptionLoggingDomainId),
                        ex
                        );
            }
            catch (Exception innerException)
            {
                Console.Write(innerException.ToString());
            }
        }
    }
}
