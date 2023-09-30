using BrassLoon.Authorization.Framework;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class TokenClaimGenerator : ITokenClaimGenerator
    {
        public async Task<IEnumerable<Claim>> Generate(ISettings settings, IUser user)
        {
            List<Claim> claims = await CreateCommonUserClaims(settings, user);
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.ReferenceId));
            AddRoleClaims(claims, await GetRoles(settings, user));
            return claims;
        }

        public async Task<IEnumerable<Claim>> Generate(ISettings settings, IClient client, IUser user = null)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString("N"))
            };
            if (user != null)
            {
                claims.AddRange(await CreateCommonUserClaims(settings, user));
            }
            AddRoleClaims(claims, await GetRoles(settings, client));
            return claims;
        }

        private static async Task<List<Claim>> CreateCommonUserClaims(ISettings settings, IUser user)
        {
            return new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, (await user.GetEmailAddress(settings)).Address),
                new Claim(JwtRegisteredClaimNames.Name, user.Name)
            };
        }

        private static async Task<List<string>> GetRoles(ISettings settings, IUser user) => await GetRoles(await user.GetRoles(settings));

        private static async Task<List<string>> GetRoles(ISettings settings, IClient client) => await GetRoles(await client.GetRoles(settings));

        private static Task<List<string>> GetRoles(IEnumerable<IRole> roles) => Task.FromResult(roles.Select(r => r.PolicyName).ToList());

        private static void AddRoleClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (string role in roles)
            {
                claims.Add(new Claim("role", role));
            }
        }
    }
}
