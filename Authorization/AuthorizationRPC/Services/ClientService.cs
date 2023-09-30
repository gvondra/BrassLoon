using BrassLoon.Authorization.Framework;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Authorization.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.Authorization.Protos;
namespace AuthorizationRPC.Services
{
    public class ClientService : Protos.ClientService.ClientServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly SettingsFactory _settingsFactory;
        private readonly IClientFactory _clientFactory;
        private readonly IClientSaver _clientSaver;
        private readonly ILogger<ClientService> _logger;
        private readonly ISecretGenerator _secretGenerator;
        private readonly IEmailAddressFactory _emailAddressFactory;

        public ClientService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            SettingsFactory settingsFactory,
            IClientFactory clientFactory,
            IClientSaver clientSaver,
            ILogger<ClientService> logger,
            ISecretGenerator secretGenerator,
            IEmailAddressFactory emailAddressFactory)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _settingsFactory = settingsFactory;
            _clientFactory = clientFactory;
            _clientSaver = clientSaver;
            _secretGenerator = secretGenerator;
            _logger = logger;
            _secretGenerator = secretGenerator;
            _emailAddressFactory = emailAddressFactory;
        }

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<Client> Create(Client request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                Validate(request);
                if (string.IsNullOrEmpty(request?.Secret))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing client secret value");
                CoreSettings settings = _settingsFactory.CreateCore();
                IClient innerClient = _clientFactory.Create(domainId, request.Secret);
                Map(request, innerClient);
                IEmailAddress userEmailAddress = await ConfigureUserEmailAddress(settings, innerClient, request.UserEmailAddress);
                await ApplyRoleChanges(settings, innerClient, request.Roles);
                await _clientSaver.Create(settings, innerClient, userEmailAddress);
                return await MapClient(settings, innerClient);
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

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<Client> Get(GetClientRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.ClientId) || !Guid.TryParse(request.ClientId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid client id \"{request?.ClientId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IClient innerClient = await _clientFactory.Get(settings, domainId, id);
                Client client = null;
                if (innerClient != null)
                {
                    client = await MapClient(settings, innerClient);
                }
                return client;
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

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task GetByDomain(GetByDomainRequest request, IServerStreamWriter<Client> responseStream, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IEnumerable<IClient> innerClients = await _clientFactory.GetByDomainId(settings, domainId);
                foreach (IClient innerClient in innerClients)
                {
                    await responseStream.WriteAsync(await MapClient(settings, innerClient));
                }
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

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override Task<ClientCredentialSecret> GetClientCredentialSecret(Empty request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(new ClientCredentialSecret
                {
                    Secret = _secretGenerator.GenerateSecret()
                });
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

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<Client> Update(Client request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.ClientId) || !Guid.TryParse(request.ClientId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid client id \"{request?.ClientId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                Validate(request);
                CoreSettings settings = _settingsFactory.CreateCore();
                IClient innerClient = await _clientFactory.Get(settings, domainId, id);
                Client result = null;
                if (innerClient != null)
                {
                    Map(request, innerClient);
                    IEmailAddress userEmailAddress = await ConfigureUserEmailAddress(settings, innerClient, request.UserEmailAddress);
                    await ApplyRoleChanges(settings, innerClient, request.Roles);
                    if (!string.IsNullOrEmpty(request?.Secret))
                        innerClient.SetSecret(request.Secret);
                    await _clientSaver.Update(settings, innerClient, userEmailAddress);
                    result = await MapClient(settings, innerClient);
                }
                return result;
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

        private async Task<IEmailAddress> ConfigureUserEmailAddress(CoreSettings settings, IClient client, string emailAddress)
        {
            IEmailAddress result = null;
            if (string.IsNullOrEmpty(emailAddress))
            {
                client.SetUserEmailAddress(null);
            }
            else
            {
                result = await _emailAddressFactory.GetByAddress(settings, emailAddress);
                client.SetUserEmailAddress(result);
                if (!result.IsNew)
                    result = null;
            }
            return result;
        }

        private static void Map(Client client, IClient innnerClient)
        {
            innnerClient.IsActive = client.IsActive ?? true;
            innnerClient.Name = client.Name;
            innnerClient.UserName = client.UserName;
        }

        private static async Task<Client> MapClient(CoreSettings settings, IClient innerClient)
        {
            Client client = new Client
            {
                ClientId = innerClient.ClientId.ToString("D"),
                CreateTimestamp = Timestamp.FromDateTime(innerClient.CreateTimestamp),
                DomainId = innerClient.DomainId.ToString("D"),
                IsActive = innerClient.IsActive,
                Name = innerClient.Name,
                UpdateTimestamp = Timestamp.FromDateTime(innerClient.UpdateTimestamp),
                UserName = innerClient.UserName,
                UserEmailAddress = (await innerClient.GetUserEmailAddress(settings))?.Address ?? string.Empty
            };
            client.Roles.Add(
                (await innerClient.GetRoles(settings)).Select<IRole, AppliedRole>(r => MapAppliedRole(r)));
            return client;
        }

        private static AppliedRole MapAppliedRole(IRole role)
        {
            return new AppliedRole
            {
                Name = role.Name,
                PolicyName = role.PolicyName
            };
        }

        private static void Validate(Client client)
        {
            if (string.IsNullOrEmpty(client?.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Missing client name value");
            if (client?.Secret != null && client.Secret.Length < 32)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Client secret must be at least 32 characters in length");
        }

        private static async Task ApplyRoleChanges(CoreSettings coreSettings, IClient innerClient, IEnumerable<AppliedRole> roles)
        {
            if (roles != null)
            {
                List<IRole> currentRoles = (await innerClient.GetRoles(coreSettings)).ToList();
                foreach (IRole currentRole in currentRoles)
                {
                    if (!roles.Any(r => string.Equals(currentRole.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerClient.RemoveRole(coreSettings, currentRole.PolicyName);
                }
                foreach (AppliedRole role in roles)
                {
                    if (!currentRoles.Any(r => string.Equals(role.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerClient.AddRole(coreSettings, role.PolicyName);
                }
            }
        }
    }
}
