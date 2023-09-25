using BrassLoon.Authorization.Framework;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Authorization.Protos;
using BrassLoon.JwtUtility;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.Authorization.Protos;

namespace AuthorizationRPC.Services
{
    public class TokenService : Protos.TokenService.TokenServiceBase
    {
        private readonly ILogger<TokenService> _logger;
        private readonly SettingsFactory _settingsFactory;
        private readonly IOptions<Settings> _settings;
        private readonly ISigningKeyFactory _signingKeyFactory;
        private readonly IClientFactory _clientFactory;
        private readonly IUserFactory _userFactory;
        private readonly IUserSaver _userSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly ITokenClaimGenerator _tokenClaimGenerator;

        public TokenService(
            ILogger<TokenService> logger,
            SettingsFactory settingsFactory,
            IOptions<Settings> settings,
            ISigningKeyFactory signingKeyFactory,
            IClientFactory clientFactory,
            IUserFactory userFactory,
            IUserSaver userSaver,
            IEmailAddressFactory emailAddressFactory,
            ITokenClaimGenerator tokenClaimGenerator)
        {
            _logger = logger;
            _settingsFactory = settingsFactory;
            _settings = settings;
            _signingKeyFactory = signingKeyFactory;
            _clientFactory = clientFactory;
            _userFactory = userFactory;
            _userSaver = userSaver;
            _emailAddressFactory = emailAddressFactory;
            _tokenClaimGenerator = tokenClaimGenerator;
        }

        [Authorize(Constants.POLICY_CREATE_TOKEN)]
        public override async Task<TokenResponse> Create(GetByDomainRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                CoreSettings settings = _settingsFactory.CreateCore();
                ISigningKey signingKey = (await _signingKeyFactory.GetByDomainId(settings, domainId))
                    .Where(sk => sk.IsActive)
                    .OrderByDescending(sk => sk.UpdateTimestamp)
                    .FirstOrDefault();
                if (signingKey == null)
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Signing key not found"), "No active signing key found");
                IUser user = await GetUser(settings, domainId);
                return new TokenResponse
                {
                    Value = await CreateToken(settings, user, signingKey)
                };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Internal System Error"), ex.Message);
            }
        }

        public override async Task<TokenResponse> CreateClientCredential(ClientCredential request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.ClientId) || !Guid.TryParse(request.ClientId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid client id \"{request?.ClientId}\"");
                if (string.IsNullOrEmpty(request?.Secret))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing client secret value");
                CoreSettings settings = _settingsFactory.CreateCore();
                ISigningKey signingKey = (await _signingKeyFactory.GetByDomainId(settings, domainId))
                    .Where(sk => sk.IsActive)
                    .OrderByDescending(sk => sk.UpdateTimestamp)
                    .FirstOrDefault();
                if (signingKey == null)
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Signing key not found"), "No active signing key found");
                IClient client = await _clientFactory.Get(settings, domainId, id);
                if (client == null || await client.AuthenticateSecret(settings, request.Secret) == false)
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                IUser user = await GetUser(settings, client);
                return new TokenResponse
                {
                    Value = await CreateToken(settings, client, signingKey, user)
                };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Internal System Error"), ex.Message);
            }
        }

        private async Task<IUser> GetUser(CoreSettings coreSettings, Guid domainId)
        {
            //IEmailAddress emailAddress = null;
            //string subscriber = GetCurrentUserReferenceId();
            //IUser user = await _userFactory.GetByReferenceId(coreSettings, domainId, subscriber);
            //if (user == null)
            //{
            //    string email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            //    emailAddress = await _emailAddressFactory.GetByAddress(coreSettings, email);
            //    user = _userFactory.Create(domainId, subscriber, emailAddress);
            //    user.Name = GetUserNameClaim().Value;
            //    await _userSaver.Create(coreSettings, user);
            //}
            //else
            //{
            //    user.Name = GetUserNameClaim().Value;
            //    await _userSaver.Update(coreSettings, user);
            //}
            //return user;
            return null;
        }

        private async Task<IUser> GetUser(CoreSettings coreSettings, IClient client)
        {
            IEmailAddress emailAddress = await client.GetUserEmailAddress(coreSettings);
            IUser user = null;
            if (emailAddress != null)
            {
                Func<BrassLoon.Authorization.Framework.ISettings, IUser, Task> saveAction = _userSaver.Update;
                user = await _userFactory.GetByEmailAddress(coreSettings, client.DomainId, emailAddress.Address);
                if (user == null)
                {
                    user = _userFactory.Create(client.DomainId, client.ClientId.ToString("N"), emailAddress);
                    saveAction = _userSaver.Create;
                }
                user.Name = client.UserName;
                await saveAction(coreSettings, user);
            }
            return user;
        }

        private async Task<string> CreateToken(CoreSettings coreSettings, IUser user, ISigningKey signingKey)
        {
            IEnumerable<Claim> claims = await _tokenClaimGenerator.Generate(coreSettings, user);
            RsaSecurityKey rsaSecurityKey = await signingKey.GetKey(coreSettings, true);
            return JwtSecurityTokenUtility.Write(
                JwtSecurityTokenUtility.Create(rsaSecurityKey, _settings.Value.TokenIssuer, _settings.Value.TokenIssuer, claims, CreateExpiration, JwtSecurityTokenUtility.CreateJwtId)
                );
        }

        private async Task<string> CreateToken(CoreSettings settings, IClient client, ISigningKey signingKey, IUser user = null)
        {
            IEnumerable<Claim> claims = await _tokenClaimGenerator.Generate(settings, client, user);
            RsaSecurityKey rsaSecurityKey = await signingKey.GetKey(settings, true);
            return JwtSecurityTokenUtility.Write(
                JwtSecurityTokenUtility.Create(rsaSecurityKey, _settings.Value.TokenIssuer, _settings.Value.TokenIssuer, claims, CreateExpiration, JwtSecurityTokenUtility.CreateJwtId)
                );
        }

        private static DateTime CreateExpiration() => DateTime.UtcNow.AddHours(6);
    }
}
