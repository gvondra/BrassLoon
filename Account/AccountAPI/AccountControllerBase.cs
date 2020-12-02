using BrassLoon.Account.Framework;
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
    }
}
