using BrassLoon.Account.Framework;
using BrassLoon.Account.Framework.Enumerations;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Account.Models;
using BrassLoon.Interface.Log;
using BrassLoon.JwtUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : AccountControllerBase
    {
        private const string JWT_ISSUER = "urn:brassloon";
        private const string JWT_AUDIENCE = "urn:brassloon";

        private readonly ILogger<TokenController> _logger;
        private readonly IAccountFactory _accountFactory;
        private readonly IClientFactory _clientFactory;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly IEmailAddressSaver _emailAddressSaver;
        private readonly ISecretProcessor _secretProcessor;
        private readonly IUserFactory _userFactory;
        private readonly IUserSaver _userSaver;

        public TokenController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<TokenController> logger,
            MapperFactory mapperFactory,
            IAccountFactory accountFactory,
            IClientFactory clientFactory,
            IEmailAddressFactory emailAddressFactory,
            IEmailAddressSaver emailAddressSaver,
            ISecretProcessor secretProcessor,
            IUserFactory userFactory,
            IUserSaver userSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory)
        {
            _logger = logger;
            _accountFactory = accountFactory;
            _clientFactory = clientFactory;
            _emailAddressFactory = emailAddressFactory;
            _emailAddressSaver = emailAddressSaver;
            _secretProcessor = secretProcessor;
            _userFactory = userFactory;
            _userSaver = userSaver;
        }

        [HttpPost]
        [Authorize("EDIT:USER")]
        public async Task<IActionResult> Create()
        {
            try
            {
                IUser user = await GetUser();
                return Ok(await CreateToken(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }            
        }

        [HttpPost("ClientCredential")]
        public async Task<IActionResult> CreateClientCredential([FromBody] ClientCredential clientCredential)
        {
            IActionResult result = null;
            try
            {
                if (result == null && clientCredential == null)
                    result = BadRequest("Missing request data");
                if (result == null && (!clientCredential.ClientId.HasValue || clientCredential.ClientId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing client id value");
                if (result == null && string.IsNullOrEmpty(clientCredential.Secret))
                    result = BadRequest("Missing secret value");
                if (result == null)
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IClient client = await _clientFactory.Get(settings, clientCredential.ClientId.Value);
                    if (client == null)
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                    {
                        if (!client.IsActive || !_secretProcessor.Verify(clientCredential.Secret, await client.GetSecretHash(settings)))
                            result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        result = Content(await CreateToken(client), "text/plain");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        private async Task<IUser> GetUser()
        {
            IUser user;
            IEmailAddress emailAddress = null;
            string subscriber = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            CoreSettings coreSettings = _settingsFactory.CreateCore(_settings.Value);
            user = await _userFactory.GetByReferenceId(coreSettings, subscriber);
            if (user == null)
            {
                string email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
                emailAddress = await _emailAddressFactory.GetByAddress(coreSettings, email);
                if (emailAddress == null)
                {
                    emailAddress = _emailAddressFactory.Create(email);
                    await _emailAddressSaver.Create(coreSettings, emailAddress);
                }
                user = _userFactory.Create(subscriber, emailAddress);
                user.Name = User.Claims
                    .First(c => string.Equals(c.Type, "name", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(c.Type, ClaimTypes.Name, StringComparison.OrdinalIgnoreCase))
                    .Value;
                SetSuperUser(user);
                await _userSaver.Create(coreSettings, user);
            }
            else
            {
                user.Name = User.Claims
                    .First(c => string.Equals(c.Type, "name", StringComparison.OrdinalIgnoreCase) 
                    || string.Equals(c.Type, ClaimTypes.Name, StringComparison.OrdinalIgnoreCase))
                    .Value;
                SetSuperUser(user);
                await _userSaver.Update(coreSettings, user);
            }
            return user;
        }
        
        [NonAction]
        private void SetSuperUser(IUser user)
        {
            string email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            if (!string.IsNullOrEmpty(_settings.Value.SuperUser) && string.Equals(email, _settings.Value.SuperUser, StringComparison.OrdinalIgnoreCase))
            {
                user.Roles = user.Roles | 
                    UserRole.SystemAdministrator |
                    UserRole.AccountAdministrator
                    ;
            }
        }

        [NonAction]
        private async Task<string> CreateToken(IUser user)
        {
            List<Claim> claims = new List<Claim>();
            Claim claim = User.Claims.FirstOrDefault(c => string.Equals(_settings.Value.ExternalIdIssuer, c.Issuer, StringComparison.OrdinalIgnoreCase) && string.Equals(ClaimTypes.NameIdentifier, c.Type, StringComparison.OrdinalIgnoreCase));
            if (claim != null)
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, claim.Value));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, (await user.GetEmailAddress(_settingsFactory.CreateCore(_settings.Value))).Address));
            claims.Add(new Claim("accounts", await GetAccountIdClaim(_accountFactory, _settingsFactory, user.UserId)));
            if ((user.Roles & UserRole.SystemAdministrator) == UserRole.SystemAdministrator)
                claims.Add(new Claim("role", "sysadmin"));
            if ((user.Roles & UserRole.AccountAdministrator) == UserRole.AccountAdministrator)
                claims.Add(new Claim("role", "actadmin"));
            return JwtSecurityTokenUtility.Write(
                JwtSecurityTokenUtility.Create(_settings.Value.TknCsp, JWT_ISSUER, JWT_AUDIENCE, claims, GetJwtExpiration)
                ); 
        }

        [NonAction]
        private async Task<string> GetAccountIdClaim(IAccountFactory accountFactory, SettingsFactory settingsFactory, Guid userId)
        {
            return string.Join(
                ' ',
                (await accountFactory.GetAccountIdsByUserId(settingsFactory.CreateCore(_settings.Value), userId))
                .Select<Guid, string>(g => g.ToString("N"))
                );
        }

        [NonAction]
        private Task<string> CreateToken(IClient client)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString("N")),
                new Claim("accounts", client.AccountId.ToString("N"))
            };
            return Task.FromResult<string>(JwtSecurityTokenUtility.Write(
                JwtSecurityTokenUtility.Create(_settings.Value.TknCsp, JWT_ISSUER, JWT_AUDIENCE, claims, GetJwtExpiration)
                ));
        }

        private static DateTime GetJwtExpiration() => DateTime.Now.AddHours(6);
    }
}
