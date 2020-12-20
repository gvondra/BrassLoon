using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LogAPI
{
    public class AuthorizationHandler : Microsoft.AspNetCore.Authorization.AuthorizationHandler<AuthorizationRequirement>, IAuthorizationHandler
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated && 
                IssuerMatches(context.User, requirement.Issuer) &&
                RoleMatches(context.User, requirement.Roles))
            {                
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
        
        private bool RoleMatches(ClaimsPrincipal user, string[] roles)
        {
            if (roles != null && roles.Length > 0)
            {
                return roles.Any(role => user.Claims.Any(
                    c => string.Equals(ClaimTypes.Role, c.Type, StringComparison.OrdinalIgnoreCase) && string.Equals(role, c.Value, StringComparison.OrdinalIgnoreCase)
                    ));
            }
            return false;
        }

        private bool IssuerMatches(ClaimsPrincipal user, string issuer)
        {
            if (!string.IsNullOrEmpty(issuer) && user.Claims.Any(c => string.Equals("iss", c.Type, StringComparison.OrdinalIgnoreCase)
                && string.Equals(issuer, c.Value, StringComparison.OrdinalIgnoreCase)))
                return true;
            return false;
        }
    }
}
