using BrassLoon.Authorization.Framework;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Authorization.Models;
using BrassLoon.Interface.Log;
using BrassLoon.JwtUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : AuthorizationContollerBase
    {
        private readonly IClientFactory _clientFactory;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly ISigningKeyFactory _signingKeyFactory;
        private readonly IUserFactory _userFactory;
        private readonly IUserSaver _userSaver;

        public TokenController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IClientFactory clientFactory,
            IEmailAddressFactory emailAddressFactory,
            ISigningKeyFactory signingKeyFactory,
            IUserFactory userFactory,
            IUserSaver userSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _clientFactory = clientFactory;
            _emailAddressFactory = emailAddressFactory;
            _signingKeyFactory = signingKeyFactory;
            _userFactory = userFactory;
            _userSaver = userSaver;
        }

        [HttpPost("{domainId}")]
        [Authorize("Create:Token")]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IUser user = await GetUser(coreSettings, domainId.Value);
                    ISigningKey signingKey = (await _signingKeyFactory.GetByDomainId(coreSettings, domainId.Value)).Where(sk => sk.IsActive)
                        .OrderByDescending(sk => sk.UpdateTimestamp)
                        .FirstOrDefault()
                        ;
                    if (signingKey != null)
                        result = Content(await CreateToken(coreSettings, user, signingKey), "text/plain");
                    else
                        result = StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpPost("ClientCredential/{domainId}")]
        public async Task<IActionResult> CreateClientCredential([FromRoute] Guid? domainId, [FromBody] ClientCredential clientCredential)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && clientCredential == null)
                    result = BadRequest("Missing request data");
                if (result == null && (!clientCredential.ClientId.HasValue || clientCredential.ClientId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing client id value");
                if (result == null && string.IsNullOrEmpty(clientCredential?.Secret))
                    result = BadRequest("Missing secret value");
                if (result == null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IClient client = await _clientFactory.Get(coreSettings, domainId.Value, clientCredential.ClientId.Value);
                    if (client == null)
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                    {
                        if (await client.AuthenticateSecret(coreSettings, clientCredential.Secret) == false)
                            result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        ISigningKey signingKey = (await _signingKeyFactory.GetByDomainId(coreSettings, domainId.Value)).Where(sk => sk.IsActive)
                            .OrderByDescending(sk => sk.UpdateTimestamp)
                            .FirstOrDefault()
                            ;
                        if (signingKey != null)
                            result = Content(await CreateToken(coreSettings, client, signingKey), "text/plain");
                        else
                            result = StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }
            catch (Exception ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [NonAction]
        private async Task<IUser> GetUser(CoreSettings coreSettings, Guid domainId)
        {
            IUser user;
            IEmailAddress emailAddress = null;
            string subscriber = GetCurrentUserReferenceId();
            user = await _userFactory.GetByReferenceId(coreSettings, domainId, subscriber);
            if (user == null)
            {
                string email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
                emailAddress = await _emailAddressFactory.GetByAddress(coreSettings, email);
                user = _userFactory.Create(domainId, subscriber, emailAddress);
                user.Name = GetUserNameClaim().Value;
                await _userSaver.Create(coreSettings, user);
            }
            else
            {
                user.Name = GetUserNameClaim().Value;
                await _userSaver.Update(coreSettings, user);
            }
            return user;
        }

        [NonAction]
        private Claim GetUserNameClaim() => User.Claims.First(c => string.Equals(c.Type, "name", StringComparison.OrdinalIgnoreCase) || string.Equals(c.Type, ClaimTypes.Name, StringComparison.OrdinalIgnoreCase));

        [NonAction]
        private async Task<List<string>> GetRoles(CoreSettings settings, IUser user) => await GetRoles(await user.GetRoles(settings));

        [NonAction]
        private async Task<List<string>> GetRoles(CoreSettings settings, IClient client) => await GetRoles(await client.GetRoles(settings));

        [NonAction]
        private Task<List<string>> GetRoles(IEnumerable<IRole> roles) => Task.FromResult(roles.Select(r => r.PolicyName).ToList());

        [NonAction]
        private void AddRoleClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (string role in roles)
            {
                claims.Add(new Claim("role", role));
            }
        }

        [NonAction]
        private async Task<string> CreateToken(CoreSettings coreSettings, IUser user, ISigningKey signingKey)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ReferenceId),
                new Claim(JwtRegisteredClaimNames.Email, (await user.GetEmailAddress(_settingsFactory.CreateCore(_settings.Value))).Address),
                new Claim(JwtRegisteredClaimNames.Name, user.Name)
            };
            AddRoleClaims(claims, await GetRoles(coreSettings, user));
            RsaSecurityKey rsaSecurityKey = await signingKey.GetKey(coreSettings, true);
            return JwtSecurityTokenUtility.Write(
                JwtSecurityTokenUtility.Create(rsaSecurityKey, _settings.Value.TokenIssuer, _settings.Value.TokenIssuer, claims, CreateExpiration, JwtSecurityTokenUtility.CreateJwtId)
                );
        }

        [NonAction]
        private async Task<string> CreateToken(CoreSettings coreSettings, IClient client, ISigningKey signingKey)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString("N"))
            };
            AddRoleClaims(claims, await GetRoles(coreSettings, client));
            RsaSecurityKey rsaSecurityKey = await signingKey.GetKey(coreSettings, true);
            return JwtSecurityTokenUtility.Write(
                JwtSecurityTokenUtility.Create(rsaSecurityKey, _settings.Value.TokenIssuer, _settings.Value.TokenIssuer, claims, CreateExpiration, JwtSecurityTokenUtility.CreateJwtId)
                );
        }

        private static DateTime CreateExpiration() => DateTime.UtcNow.AddHours(6);
    }
}
