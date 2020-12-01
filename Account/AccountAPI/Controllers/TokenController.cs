using Autofac;
using BrassLoon.Account.Framework;
using BrassLoon.Interface.Account.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly IContainer _container;

        public TokenController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
        }

        [HttpPost]
        [Authorize("EDIT:USER")]
        public async Task<IActionResult> Create()
        {
            IUser user = await GetUser();
            return Ok(await CreateToken(user));
        }

        [HttpPost("ClientCredential")]
        public async Task<IActionResult> CreateClientCredential([FromBody] ClientCredential clientCredential)
        {
            IActionResult result = null;
            if (result == null && clientCredential == null)
                result = BadRequest("Missing request data");
            if (result == null && (!clientCredential.ClientId.HasValue || clientCredential.ClientId.Value.Equals(Guid.Empty)))
                result = BadRequest("Missing client id value");
            if (result == null && string.IsNullOrEmpty(clientCredential.Secret))
                result = BadRequest("Missing secret value");
            if (result == null)
            {
                using ILifetimeScope scope = _container.BeginLifetimeScope();
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                IClientFactory clientFactory = scope.Resolve<IClientFactory>();
                IClient client = await clientFactory.Get(settings, clientCredential.ClientId.Value);
                if (client == null)
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    ISecretProcessor secretProcessor = scope.Resolve<ISecretProcessor>();
                    if (!secretProcessor.Verify(clientCredential.Secret, await client.GetSecretHash(settings)))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                if (result == null)
                {
                    result = Ok(await CreateToken(client));
                }
            }
            return result;
        }

        [NonAction]
        private async Task<IUser> GetUser()
        {
            IUser user;
            IEmailAddress emailAddress = null;
            using (ILifetimeScope scope = _container.BeginLifetimeScope())
            {
                string subscriber = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                IUserFactory userFactory = scope.Resolve<IUserFactory>();
                IEmailAddressFactory emailAddressFactory = scope.Resolve<IEmailAddressFactory>();
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                user = await userFactory.GetByReferenceId(settingsFactory.CreateAccount(_settings.Value), subscriber);
                if (user == null)
                {
                    string email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
                    emailAddress = await emailAddressFactory.GetByAddress(settingsFactory.CreateAccount(_settings.Value), email);
                    if (emailAddress == null)
                    {
                        emailAddress = emailAddressFactory.Create(email);
                        IEmailAddressSaver emailAddressSaver = scope.Resolve<IEmailAddressSaver>();
                        await emailAddressSaver.Create(settingsFactory.CreateAccount(_settings.Value), emailAddress);
                    }
                    user = userFactory.Create(subscriber, emailAddress);
                    user.Name = User.Claims.First(c => string.Equals(c.Type, "name", StringComparison.OrdinalIgnoreCase)).Value;
                    IUserSaver userSaver = scope.Resolve<IUserSaver>();
                    await userSaver.Create(settingsFactory.CreateAccount(_settings.Value), user);
                }
            }
            return user;
        }

        [NonAction]
        private async Task<string> CreateToken(IUser user)
        {
            using (ILifetimeScope scope = _container.BeginLifetimeScope())
            {
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                RSAParameters rsaParameters = CreateRSAParameter();
                //RSA rsa = new RSACryptoServiceProvider(2048);
                //Debug.WriteLine(Convert.ToBase64String(rsa.ExportRSAPublicKey()));
                RsaSecurityKey securityKey = new RsaSecurityKey(rsaParameters);
                JsonWebKey jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(securityKey);
                //Debug.WriteLine(JsonConvert.SerializeObject(jsonWebKey));
                SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha512);
                List<Claim> claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
                };
                Claim claim = User.Claims.FirstOrDefault(c => string.Equals(_settings.Value.IdIssuer, c.Issuer, StringComparison.OrdinalIgnoreCase) && string.Equals(ClaimTypes.NameIdentifier, c.Type, StringComparison.OrdinalIgnoreCase));
                if (claim != null)
                    claims.Add(new Claim(JwtRegisteredClaimNames.Sub, claim.Value));
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, (await user.GetEmailAddress(settingsFactory.CreateAccount(_settings.Value))).Address));
                claims.Add(new Claim("accounts", await GetAccountIdClaim(scope.Resolve<IAccountFactory>(), settingsFactory, user.UserId)));
                JwtSecurityToken token = new JwtSecurityToken(
                    "urn:brassloon",
                    "urn:brassloon",
                    claims,
                    expires: DateTime.Now.AddHours(6),
                    signingCredentials: credentials
                    );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }                        
        }

        [NonAction]
        private async Task<string> GetAccountIdClaim(IAccountFactory accountFactory, SettingsFactory settingsFactory, Guid userId)
        {
            return string.Join(
                ' ',
                (await accountFactory.GetAccountIdsByUserId(settingsFactory.CreateAccount(_settings.Value), userId))
                .Select<Guid, string>(g => g.ToString("N"))
                );
        }

        [NonAction]
        private Task<string> CreateToken(IClient client)
        {
            using ILifetimeScope scope = _container.BeginLifetimeScope();
            SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
            RSAParameters rsaParameters = CreateRSAParameter();
            //RSA rsa = new RSACryptoServiceProvider(2048);
            //Debug.WriteLine(Convert.ToBase64String(rsa.ExportRSAPublicKey()));
            RsaSecurityKey securityKey = new RsaSecurityKey(rsaParameters);
            JsonWebKey jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(securityKey);
            //Debug.WriteLine(JsonConvert.SerializeObject(jsonWebKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha512);
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            };
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString("N")));
            claims.Add(new Claim("accounts", client.AccountId.ToString("N")));
            JwtSecurityToken token = new JwtSecurityToken(
                "urn:brassloon",
                "urn:brassloon",
                claims,
                expires: DateTime.Now.AddHours(6),
                signingCredentials: credentials
                );
            return Task.FromResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }

        [NonAction]
        private RSAParameters CreateRSAParameter()
        {
            dynamic tknCsp = JsonConvert.DeserializeObject(_settings.Value.TknCsp);
            return new RSAParameters
            {
                D = Base64UrlEncoder.DecodeBytes((string)tknCsp.d),
                DP = Base64UrlEncoder.DecodeBytes((string)tknCsp.dp),
                DQ = Base64UrlEncoder.DecodeBytes((string)tknCsp.dq),
                Exponent = Base64UrlEncoder.DecodeBytes((string)tknCsp.e),
                InverseQ = Base64UrlEncoder.DecodeBytes((string)tknCsp.qi),
                Modulus = Base64UrlEncoder.DecodeBytes((string)tknCsp.n),
                P = Base64UrlEncoder.DecodeBytes((string)tknCsp.p),
                Q = Base64UrlEncoder.DecodeBytes((string)tknCsp.q)
            };
        }
    }
}
