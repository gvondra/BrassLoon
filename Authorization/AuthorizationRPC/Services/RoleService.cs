using BrassLoon.Authorization.Framework;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Authorization.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.Authorization.Protos;

namespace AuthorizationRPC.Services
{
    public class RoleService : Protos.RoleService.RoleServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILogger<ClientService> _logger;
        private readonly IRoleFactory _roleFactory;
        private readonly IRoleSaver _roleSaver;

        public RoleService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            SettingsFactory settingsFactory,
            ILogger<ClientService> logger,
            IRoleFactory roleFactory,
            IRoleSaver roleSaver)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _roleFactory = roleFactory;
            _roleSaver = roleSaver;
        }

        public override async Task<Role> Create(Role request, ServerCallContext context)
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
                ValidateCreate(request);
                CoreSettings settings = _settingsFactory.CreateCore();
                await ValidatePolicyNameNotExists(settings, domainId, request);
                IRole innerRole = _roleFactory.Create(domainId, request.PolicyName);
                _ = Map(request, innerRole);
                await _roleSaver.Create(settings, innerRole);
                return Map(innerRole);
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

        public override async Task GetByDomainId(GetByDomainRequest request, IServerStreamWriter<Role> responseStream, ServerCallContext context)
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
                IEnumerable<IRole> innerRoles = await _roleFactory.GetByDomainId(settings, domainId);
                foreach (IRole innerRole in innerRoles)
                {
                    await responseStream.WriteAsync(Map(innerRole));
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

        public override async Task<Role> Update(Role request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.RoleId) || !Guid.TryParse(request.RoleId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid role id \"{request?.RoleId}\"");
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
                IRole innerRole = await _roleFactory.Get(settings, domainId, id);
                if (innerRole == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Role Not Found"));
                _ = Map(request, innerRole);
                await _roleSaver.Update(settings, innerRole);
                return Map(innerRole);
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

        private static IRole Map(Role role, IRole innerRole)
        {
            innerRole.IsActive = role.IsActive ?? true;
            innerRole.Comment = role.Comment ?? string.Empty;
            innerRole.Name = role.Name ?? string.Empty;
            return innerRole;
        }

        private static Role Map(IRole innerRole)
        {
            return new Protos.Role
            {
                Comment = innerRole.Comment,
                CreateTimestamp = Timestamp.FromDateTime(innerRole.CreateTimestamp),
                DomainId = innerRole.DomainId.ToString("D"),
                IsActive = innerRole.IsActive,
                Name = innerRole.Name,
                PolicyName = innerRole.PolicyName,
                RoleId = innerRole.RoleId.ToString("D"),
                UpdateTimestamp = Timestamp.FromDateTime(innerRole.UpdateTimestamp)
            };
        }

        private static void ValidateCreate(Role role)
        {
            Validate(role);
            if (string.IsNullOrEmpty(role.PolicyName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Missing role policy name value");
        }

        private static void Validate(Role role)
        {
            if (string.IsNullOrEmpty(role?.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Missing role name value");
        }

        private async Task ValidatePolicyNameNotExists(CoreSettings coreSettings, Guid domainId, Role role)
        {
            if ((await _roleFactory.GetByDomainId(coreSettings, domainId)).Any(r => string.Equals(role.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                throw new RpcException(new Status(StatusCode.AlreadyExists, "Role Already Exits"), $"A role with policy name \"{role.PolicyName}\" already exists");
        }
    }
}
