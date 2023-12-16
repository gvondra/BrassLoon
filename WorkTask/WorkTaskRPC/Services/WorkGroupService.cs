using BrassLoon.CommonAPI;
using BrassLoon.Interface.WorkTask.Protos;
using BrassLoon.WorkTask.Framework;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.WorkTask.Protos;

namespace WorkTaskRPC.Services
{
    public class WorkGroupService : Protos.WorkGroupService.WorkGroupServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILogger<WorkGroupService> _logger;
        private readonly IWorkGroupFactory _workGroupFactory;
        private readonly IWorkGroupSaver _workGroupSaver;

        public WorkGroupService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            SettingsFactory settingsFactory,
            ILogger<WorkGroupService> logger,
            IWorkGroupFactory workGroupFactory,
            IWorkGroupSaver workGroupSaver)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workGroupFactory = workGroupFactory;
            _workGroupSaver = workGroupSaver;
        }


        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<Empty> AddWorkTaskTypeLink(WorkGroupTaskTypeLinkRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                Guid workTaskTypeId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.WorkGroupId) || !Guid.TryParse(request.WorkGroupId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work group id \"{request.WorkGroupId}\"");
                if (string.IsNullOrEmpty(request.WorkTaskTypeId) || !Guid.TryParse(request.WorkTaskTypeId, out workTaskTypeId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work task type id \"{request.WorkTaskTypeId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                await _workGroupSaver.CreateWorkTaskTypeGroup(settings, domainId, workTaskTypeId, id);
                return new Empty();
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
        public override async Task<Empty> DeleteWorkTaskTypeLink(WorkGroupTaskTypeLinkRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                Guid workTaskTypeId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.WorkGroupId) || !Guid.TryParse(request.WorkGroupId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work group id \"{request.WorkGroupId}\"");
                if (string.IsNullOrEmpty(request.WorkTaskTypeId) || !Guid.TryParse(request.WorkTaskTypeId, out workTaskTypeId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work task type id \"{request.WorkTaskTypeId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                await _workGroupSaver.DeleteWorkTaskTypeGroup(settings, domainId, workTaskTypeId, id);
                return new Empty();
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
        public override async Task<WorkGroup> Get(GetWorkGroupRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.WorkGroupId) || !Guid.TryParse(request.WorkGroupId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work group id \"{request.WorkGroupId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IWorkGroup innerWorkGroup = await _workGroupFactory.Get(settings, domainId, id);
                return innerWorkGroup != null ? Map(innerWorkGroup) : null;
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
        public override async Task GetAll(GetByDomainRequest request, IServerStreamWriter<WorkGroup> responseStream, ServerCallContext context)
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
                IEnumerable<IWorkGroup> innerWorkGroups = await _workGroupFactory.GetByDomainId(settings, domainId);
                foreach (WorkGroup workGroup in innerWorkGroups.Select(Map))
                {
                    await responseStream.WriteAsync(workGroup);
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
        public override async Task GetByMemberUserId(GetWorkGroupByMeemberUserIdRequest request, IServerStreamWriter<WorkGroup> responseStream, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.UserId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing member user id value");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IEnumerable<IWorkGroup> innerWorkGroups = await _workGroupFactory.GetByMemberUserId(settings, domainId, request.UserId);
                foreach (WorkGroup workGroup in innerWorkGroups.Select(Map))
                {
                    await responseStream.WriteAsync(workGroup);
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
        public override async Task<WorkGroup> Create(WorkGroup request, ServerCallContext context)
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
                CoreSettings settings = _settingsFactory.CreateCore();
                IWorkGroup innerWorkGroup = _workGroupFactory.Create(domainId);
                Map(request, innerWorkGroup);
                ApplyMemberChanges(innerWorkGroup, request.MemberUserIds);
                await _workGroupSaver.Create(settings, innerWorkGroup);
                return Map(innerWorkGroup);
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
        public override async Task<WorkGroup> Update(WorkGroup request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.WorkGroupId) || !Guid.TryParse(request.WorkGroupId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work group id \"{request.WorkGroupId}\"");
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
                IWorkGroup innerWorkGroup = await _workGroupFactory.Get(settings, domainId, id);
                if (innerWorkGroup == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Work Group Not Found"));
                Map(request, innerWorkGroup);
                ApplyMemberChanges(innerWorkGroup, request.MemberUserIds);
                await _workGroupSaver.Update(settings, innerWorkGroup);
                return Map(innerWorkGroup);
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

        private static void Validate(WorkGroup workGroup)
        {
            if (string.IsNullOrEmpty(workGroup?.Title))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Missing work group title value");
        }

        private static void ApplyMemberChanges(IWorkGroup innerWorkGroup, IEnumerable<string> memberUserIds)
        {
            if (memberUserIds != null)
            {
                foreach (string userId in memberUserIds)
                {
                    // iwork group will ellimate duplicates
                    innerWorkGroup.AddMember(userId);
                }
                foreach (string userId in innerWorkGroup.MemberUserIds)
                {
                    if (!memberUserIds.Any(id => string.Equals(id, userId, StringComparison.OrdinalIgnoreCase)))
                    {
                        innerWorkGroup.RemoveMember(userId);
                    }
                }
            }
        }

        private static void Map(WorkGroup workGroup, IWorkGroup innerWorkGroup)
        {
            innerWorkGroup.Description = workGroup.Description;
            innerWorkGroup.Title = workGroup.Title;
        }

        private static WorkGroup Map(IWorkGroup innerWorkGroup)
        {
            WorkGroup result = new WorkGroup
            {
                CreateTimestamp = Timestamp.FromDateTime(innerWorkGroup.CreateTimestamp),
                Description = innerWorkGroup.Description,
                DomainId = innerWorkGroup.DomainId.ToString("D"),
                Title = innerWorkGroup.Title,
                UpdateTimestamp = Timestamp.FromDateTime(innerWorkGroup.UpdateTimestamp),
                WorkGroupId = innerWorkGroup.WorkGroupId.ToString("D"),
            };
            foreach (string memberUserId in innerWorkGroup.MemberUserIds)
            {
                result.MemberUserIds.Add(memberUserId);
            }
            foreach (string id in innerWorkGroup.WorkTaskTypeIds.Select(id => id.ToString("D")))
            {
                result.WorkTaskTypeIds.Add(id);
            }
            return result;
        }
    }
}
