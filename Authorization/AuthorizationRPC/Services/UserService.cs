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
using System.Security.Claims;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.Authorization.Protos;

namespace AuthorizationRPC.Services
{
    public class UserService : Protos.UserService.UserServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILogger<UserService> _logger;
        private readonly IUserFactory _userFactory;
        private readonly IUserSaver _userSaver;

        public UserService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            SettingsFactory settingsFactory,
            ILogger<UserService> logger,
            IUserFactory userFactory,
            IUserSaver userSaver)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _userFactory = userFactory;
            _userSaver = userSaver;
        }

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<User> Get(GetUserRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.UserId) || !Guid.TryParse(request.UserId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid user id \"{request.UserId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IUser innerUser = await _userFactory.Get(settings, domainId, id);
                User user = null;
                if (innerUser != null)
                    user = await Map(settings, innerUser);
                return user;
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
        public override async Task<GetUserNameResponse> GetName(GetUserRequest request, ServerCallContext context)
        {
            try
            {
                User user = await Get(request, context);
                return new GetUserNameResponse
                {
                    Name = user?.Name
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

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task GetByDomain(GetByDomainRequest request, IServerStreamWriter<User> responseStream, ServerCallContext context)
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
                IEnumerable<IUser> innerUsers = await _userFactory.GetByDomainId(settings, domainId);
                foreach (Task<User> getUser in innerUsers.Select(u => Map(settings, u)))
                {
                    await responseStream.WriteAsync(await getUser);
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
        public override async Task Search(SearchUserRequest request, IServerStreamWriter<User> responseStream, ServerCallContext context)
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
                IUser innerUser = null;
                List<IUser> innerUsers = null;
                if (!string.IsNullOrEmpty(request.EmailAddress))
                {
                    innerUser = await _userFactory.GetByEmailAddress(settings, domainId, request.EmailAddress);
                }
                else if (!string.IsNullOrEmpty(request.ReferenceId))
                {
                    innerUser = await _userFactory.GetByReferenceId(settings, domainId, request.ReferenceId);
                }
                else
                {
                    innerUser = await _userFactory.GetByReferenceId(settings, domainId, GetReferenceId(context.GetHttpContext().User));
                }
                if (innerUser != null && innerUsers == null)
                    innerUsers = new List<IUser> { innerUser };
                else if (innerUsers == null)
                    innerUsers = new List<IUser>();
                foreach (Task<User> getUser in innerUsers.Select(u => Map(settings, u)))
                {
                    await responseStream.WriteAsync(await getUser);
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
        public override async Task<User> Update(User request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.UserId) || !Guid.TryParse(request.UserId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid user id \"{request.UserId}\"");
                if (string.IsNullOrEmpty(request.Name))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing missing user name value");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IUser innerUser = await _userFactory.Get(settings, domainId, id);
                User user = null;
                if (innerUser != null)
                {
                    Map(request, innerUser);
                    await ApplyRoleChanges(settings, request.Roles, innerUser);
                    await _userSaver.Update(settings, innerUser);
                    user = await Map(settings, innerUser);
                }
                return user;
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

        private static string GetReferenceId(ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        private static async Task ApplyRoleChanges(CoreSettings settings, IEnumerable<AppliedRole> appliedRoles, IUser innerUser)
        {
            if (appliedRoles != null)
            {
                List<IRole> currentRoles = (await innerUser.GetRoles(settings)).ToList();
                foreach (IRole currentRole in currentRoles)
                {
                    if (!appliedRoles.Any(r => string.Equals(currentRole.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerUser.RemoveRole(settings, currentRole.PolicyName);
                }
                foreach (AppliedRole role in appliedRoles)
                {
                    if (!currentRoles.Exists(r => string.Equals(role.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerUser.AddRole(settings, role.PolicyName);
                }
            }
        }

        private static void Map(User user, IUser innerUser)
            => innerUser.Name = user.Name;

        private static async Task<User> Map(CoreSettings settings, IUser innerUser)
        {
            User user = new User
            {
                CreateTimestamp = Timestamp.FromDateTime(innerUser.CreateTimestamp),
                DomainId = innerUser.DomainId.ToString("D"),
                EmailAddress = (await innerUser.GetEmailAddress(settings))?.Address ?? string.Empty,
                Name = innerUser.Name,
                ReferenceId = innerUser.ReferenceId,
                UpdateTimestamp = Timestamp.FromDateTime(innerUser.UpdateTimestamp),
                UserId = innerUser.UserId.ToString("D")
            };
            foreach (IRole innerRole in await innerUser.GetRoles(settings))
            {
                user.Roles.Add(Map(innerRole));
            }
            return user;
        }

        private static AppliedRole Map(IRole innerRole)
        {
            return new AppliedRole
            {
                Name = innerRole.Name,
                PolicyName = innerRole.PolicyName
            };
        }
    }
}
