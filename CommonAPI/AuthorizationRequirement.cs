using Microsoft.AspNetCore.Authorization;
namespace BrassLoon.CommonAPI
{
    public class AuthorizationRequirement : IAuthorizationRequirement
    {
        public AuthorizationRequirement(string policyName, string issuer)
        {
            this.PolicyName = policyName;
            this.Issuer = issuer;
            this.Roles = new string[] { };
        }

        public AuthorizationRequirement(string policyName, string issuer, params string[] roles)
        {
            this.PolicyName = policyName;
            this.Issuer = issuer;
            this.Roles = roles;
        }

        public string PolicyName { get; set; }
        public string Issuer { get; set; }
        public string[] Roles { get; set; }
    }
}
