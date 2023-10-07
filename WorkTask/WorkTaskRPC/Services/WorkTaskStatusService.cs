using BrassLoon.CommonAPI;
using BrassLoon.Interface.WorkTask.Protos;
using BrassLoon.WorkTask.Framework;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.WorkTask.Protos;
using System.Linq;

namespace WorkTaskRPC.Services
{
    public class WorkTaskStatusService : Protos.WorkTaskStatusService.WorkTaskStatusServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILogger<WorkTaskStatusService> _logger;
        private readonly IWorkTaskStatusFactory _workTaskStatusFactory;
        private readonly IWorkTaskTypeFactory _workTaskTypeFactory;
        private readonly IWorkTaskTypeSaver _workTaskTypeSaver;

        public WorkTaskStatusService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            SettingsFactory settingsFactory,
            ILogger<WorkTaskStatusService> logger,
            IWorkTaskStatusFactory workTaskStatusFactory,
            IWorkTaskTypeFactory workTaskTypeFactory,
            IWorkTaskTypeSaver workTaskTypeSaver)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workTaskStatusFactory = workTaskStatusFactory;
            _workTaskTypeFactory = workTaskTypeFactory;
            _workTaskTypeSaver = workTaskTypeSaver;
        }

        public override async Task GetAll(GetAllWorkTaskStatusRequest request, IServerStreamWriter<WorkTaskStatus> responseStream, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid workTaskTypeId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskTypeId) || !Guid.TryParse(request.WorkTaskTypeId, out workTaskTypeId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or work task type id \"{request?.WorkTaskTypeId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IEnumerable<IWorkTaskStatus> innerWorkTaskStatuses = await _workTaskStatusFactory.GetByWorkTaskTypeId(settings, domainId, workTaskTypeId);
                foreach (WorkTaskStatus workTaskStatus in innerWorkTaskStatuses.Select(wts => Map(wts)))
                {
                    await responseStream.WriteAsync(workTaskStatus);
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

        public override async Task<WorkTaskStatus> Get(GetWorkTaskStatusRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid workTaskTypeId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskTypeId) || !Guid.TryParse(request.WorkTaskTypeId, out workTaskTypeId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or work task type id \"{request?.WorkTaskTypeId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskStatusId) || !Guid.TryParse(request.WorkTaskStatusId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or work task status id \"{request?.WorkTaskStatusId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IWorkTaskStatus innerWorkTaskStatus = await _workTaskStatusFactory.Get(settings, domainId, id);
                WorkTaskStatus workTaskStatus = null;
                if (innerWorkTaskStatus != null)
                    workTaskStatus = Map(innerWorkTaskStatus);
                return workTaskStatus;
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
        public override async Task<WorkTaskStatus> Create(WorkTaskStatus request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid workTaskTypeId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskTypeId) || !Guid.TryParse(request.WorkTaskTypeId, out workTaskTypeId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or work task type id \"{request?.WorkTaskTypeId}\"");
                if (string.IsNullOrEmpty(request?.Code))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing work task status code value");
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
                IWorkTaskType innerWorkTaskType = await _workTaskTypeFactory.Get(settings, domainId, workTaskTypeId);
                if (innerWorkTaskType == null)
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Not Found"), $"Work task type \"{request.WorkTaskTypeId}\" not found");
                IWorkTaskStatus innerWorkTaskStatus = innerWorkTaskType.CreateWorkTaskStatus(request.Code);
                Map(request, innerWorkTaskStatus);
                await _workTaskTypeSaver.Create(settings, innerWorkTaskStatus);
                return Map(innerWorkTaskStatus);
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

        public override async Task<WorkTaskStatus> Update(WorkTaskStatus request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid workTaskTypeId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskTypeId) || !Guid.TryParse(request.WorkTaskTypeId, out workTaskTypeId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or work task type id \"{request?.WorkTaskTypeId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskStatusId) || !Guid.TryParse(request.WorkTaskStatusId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or work task status id \"{request?.WorkTaskStatusId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IWorkTaskType innerWorkTaskType = await _workTaskTypeFactory.Get(settings, domainId, workTaskTypeId);
                if (innerWorkTaskType == null)
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Not Found"), $"Work task type \"{request.WorkTaskTypeId}\" not found");
                IWorkTaskStatus innerWorkTaskStatus = await _workTaskStatusFactory.Get(settings, domainId, id);
                if (innerWorkTaskStatus == null || innerWorkTaskType.WorkTaskTypeId != innerWorkTaskStatus.WorkTaskTypeId)
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Not Found"), $"Work task status \"{id}\" not found");
                Map(request, innerWorkTaskStatus);
                await _workTaskTypeSaver.Update(settings, innerWorkTaskStatus);
                return Map(innerWorkTaskStatus);
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

        public override async Task<Empty> Delete(DateWorkTaskStatusRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid workTaskTypeId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskTypeId) || !Guid.TryParse(request.WorkTaskTypeId, out workTaskTypeId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or work task type id \"{request?.WorkTaskTypeId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskStatusId) || !Guid.TryParse(request.WorkTaskStatusId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or work task status id \"{request?.WorkTaskStatusId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IWorkTaskType innerWorkTaskType = await _workTaskTypeFactory.Get(settings, domainId, workTaskTypeId);
                IWorkTaskStatus innerWorkTaskStatus = await _workTaskStatusFactory.Get(settings, domainId, id);
                if (innerWorkTaskType != null
                    && innerWorkTaskStatus != null
                    && innerWorkTaskType.WorkTaskTypeId == innerWorkTaskStatus.WorkTaskTypeId)
                {
                    if (innerWorkTaskStatus.WorkTaskCount > 0)
                        throw new RpcException(new Status(StatusCode.FailedPrecondition, "Bad Request"), "Unable to delete status is in use");
                    else
                        await _workTaskTypeSaver.DeleteStatus(settings, id);
                }
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

        private static void Validate(WorkTaskStatus workTaskStatus)
        {
            if (string.IsNullOrEmpty(workTaskStatus?.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Missing work task status name value");
            if (!workTaskStatus.IsClosedStatus.HasValue)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Missing work task status is closed value");
        }

        private static void Map(WorkTaskStatus workTaskStatus, IWorkTaskStatus innerWorkTaskStatus)
        {
            innerWorkTaskStatus.IsDefaultStatus = workTaskStatus.IsDefaultStatus ?? false;
            innerWorkTaskStatus.Description = workTaskStatus.Description;
            innerWorkTaskStatus.IsClosedStatus = workTaskStatus.IsClosedStatus.Value;
            innerWorkTaskStatus.Name = workTaskStatus.Name;
        }

        internal static WorkTaskStatus Map(IWorkTaskStatus innerWorkTaskStatus)
        {
            return new WorkTaskStatus
            {
                Code = innerWorkTaskStatus.Code,
                CreateTimestamp = Timestamp.FromDateTime(innerWorkTaskStatus.CreateTimestamp),
                Description = innerWorkTaskStatus.Description,
                DomainId = innerWorkTaskStatus.DomainId.ToString("D"),
                IsClosedStatus = innerWorkTaskStatus.IsClosedStatus,
                IsDefaultStatus = innerWorkTaskStatus.IsDefaultStatus,
                Name = innerWorkTaskStatus.Name,
                UpdateTimestamp = Timestamp.FromDateTime(innerWorkTaskStatus.UpdateTimestamp),
                WorkTaskCount = innerWorkTaskStatus.WorkTaskCount,
                WorkTaskStatusId = innerWorkTaskStatus.WorkTaskStatusId.ToString("D"),
                WorkTaskTypeId = innerWorkTaskStatus.WorkTaskTypeId.ToString("D")
            };
        }
    }
}
