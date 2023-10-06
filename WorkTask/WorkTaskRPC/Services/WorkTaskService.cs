using BrassLoon.CommonAPI;
using BrassLoon.Interface.WorkTask.Protos;
using BrassLoon.WorkTask.Framework;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.WorkTask.Protos;

namespace WorkTaskRPC.Services
{
    public class WorkTaskService : Protos.WorkTaskService.WorkTaskServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILogger<WorkTaskService> _logger;
        private readonly IWorkTaskFactory _workTaskFactory;
        private readonly IWorkTaskSaver _workTaskSaver;
        private readonly IWorkTaskTypeFactory _workTaskTypeFactory;
        private readonly IWorkTaskStatusFactory _workTaskStatusFactory;
        private readonly IWorkTaskPatcher _workTaskPatcher;

        public WorkTaskService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            SettingsFactory settingsFactory,
            ILogger<WorkTaskService> logger,
            IWorkTaskFactory workTaskFactory,
            IWorkTaskSaver workTaskSaver,
            IWorkTaskTypeFactory workTaskTypeFactory,
            IWorkTaskStatusFactory workTaskStatusFactory,
            IWorkTaskPatcher workTaskPatcher)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workTaskFactory = workTaskFactory;
            _workTaskSaver = workTaskSaver;
            _workTaskTypeFactory = workTaskTypeFactory;
            _workTaskStatusFactory = workTaskStatusFactory;
            _workTaskPatcher = workTaskPatcher;
        }

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<ClaimWorkTaskResponse> Claim(ClaimWorkTaskRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskId) || !Guid.TryParse(request?.WorkTaskId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work task type id \"{request?.WorkTaskId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IWorkTask innerWorkTask = await _workTaskFactory.Get(settings, domainId, id);
                if (innerWorkTask == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Work Task Not Found"));
                DateTime? assignedDate = !string.IsNullOrEmpty(request.AssignedDate) ? DateTime.Parse(request.AssignedDate, CultureInfo.InvariantCulture) : default;
                ClaimWorkTaskResponse response = null;
                if (!string.IsNullOrEmpty(request.AssignToUserId)
                    && !string.IsNullOrEmpty(innerWorkTask.AssignedToUserId)
                    && !string.Equals(request.AssignToUserId, innerWorkTask.AssignedToUserId, StringComparison.OrdinalIgnoreCase))
                {
                    response = new ClaimWorkTaskResponse
                    {
                        IsAssigned = false,
                        Message = "Work task already assigned"
                    };
                }
                if (response == null && string.IsNullOrEmpty(request.AssignToUserId) && assignedDate.HasValue)
                {
                    assignedDate = null;
                }
                else if (response == null && !string.IsNullOrEmpty(request.AssignToUserId) && !assignedDate.HasValue)
                {
                    assignedDate = DateTime.Today;
                }
                if (response == null)
                {
                    response.IsAssigned = await _workTaskSaver.Claim(settings, domainId, id, request.AssignToUserId, assignedDate);
                    if (response.IsAssigned)
                    {
                        response.Message = "Work task assigned";
                        response.AssignedToUserId = request.AssignToUserId;
                        response.AssignedDate = assignedDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;
                    }
                    else
                    {
                        response.Message = "Failed to assign work task";
                    }
                }
                return response;
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
        public override async Task<WorkTask> Create(WorkTask request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid workTaskTypeId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskType?.WorkTaskTypeId) || !Guid.TryParse(request?.WorkTaskType?.WorkTaskTypeId, out workTaskTypeId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing work task type value");
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
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Work Task Type Not Found"), $"Work task type not found ({workTaskTypeId})");
                IWorkTaskStatus innerWorkTaskStatus = (await _workTaskStatusFactory.GetByWorkTaskTypeId(settings, domainId, innerWorkTaskType.WorkTaskTypeId))
                    .FirstOrDefault(s => s.WorkTaskStatusId.Equals(Guid.Parse(request.WorkTaskStatus.WorkTaskStatusId)));
                if (innerWorkTaskStatus == null)
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Status Not Found"), "Invalid work task status. The status doesn't exist or is not valid for the task type");
                IWorkTask innerWorkTask = _workTaskFactory.Create(domainId, innerWorkTaskType, innerWorkTaskStatus);
                Map(request, innerWorkTask);
                ApplyContexts(request, innerWorkTask);
                await _workTaskSaver.Create(settings, innerWorkTask);
                return Map(innerWorkTask);
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
        public override async Task<WorkTask> Get(GetWorkTaskRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskId) || !Guid.TryParse(request.WorkTaskId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work task id \"{request?.WorkTaskId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IWorkTask innerWorkTask = await _workTaskFactory.Get(settings, domainId, id);
                if (innerWorkTask == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));
                return Map(innerWorkTask);
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
        public override async Task GetAll(GetByDomainRequest request, IServerStreamWriter<WorkTask> responseStream, ServerCallContext context)
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
                await foreach (IWorkTask innerWorkTask in await _workTaskFactory.GetAll(settings, domainId))
                {
                    await responseStream.WriteAsync(Map(innerWorkTask));
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
        public override async Task GetByContext(GetWorkTaskByContextRequest request, IServerStreamWriter<WorkTask> responseStream, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.ReferenceValue))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing reference value");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                foreach (IWorkTask innerWorkTask in await _workTaskFactory.GetByContextReference(settings, domainId, (short)request.ReferenceType, request.ReferenceValue, request.IncludeClosed))
                {
                    await responseStream.WriteAsync(Map(innerWorkTask));
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
        public override async Task GetByWorkGroupId(GetWorkTaskByWorkGroupIdRequest request, IServerStreamWriter<WorkTask> responseStream, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid workGroupId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.WorkGroupId) || !Guid.TryParse(request.WorkGroupId, out workGroupId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work group id \"{request?.WorkGroupId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                foreach (IWorkTask innerWorkTask in await _workTaskFactory.GetByWorkGroupId(settings, domainId, workGroupId, request.IncludeClosed))
                {
                    await responseStream.WriteAsync(Map(innerWorkTask));
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
        public override async Task Patch(IAsyncStreamReader<PatchWorkTaskRequest> requestStream, IServerStreamWriter<WorkTask> responseStream, ServerCallContext context)
        {
            try
            {
                SortedSet<Guid> validDomains = new SortedSet<Guid>();
                Guid domainId;
                CoreSettings settings = _settingsFactory.CreateCore();
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                while (await requestStream.MoveNext())
                {
                    if (string.IsNullOrEmpty(requestStream.Current.DomainId) || !Guid.TryParse(requestStream.Current.DomainId, out domainId))
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{requestStream.Current.DomainId}\"");
                    if (!validDomains.Contains(domainId) && !await _domainAcountAccessVerifier.HasAccess(
                        _settingsFactory.CreateAccount(accessToken),
                        domainId,
                        accessToken))
                    {
                        throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                    }
                    validDomains.Add(domainId);
                    ValidatePatchData(requestStream.Current.Data);
                    IWorkTask innerWorkTask = await _workTaskPatcher.Apply(settings, domainId, requestStream.Current.Data);
                    await _workTaskSaver.Update(settings, innerWorkTask);
                    await responseStream.WriteAsync(Map(innerWorkTask));
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

        private static void ValidatePatchData(IDictionary<string, string> patchData)
        {
            int i;
            string[] required = new string[] { "WorkTaskId" };
            string[] validFields = new string[] { "WorkTaskStatusId" };
            string fields = string.Join(
                ", ",
                patchData.Keys
                .Where(fld => !required.Concat(validFields).Contains(fld))
                .Distinct()
                .OrderBy(fld => fld));
            if (!string.IsNullOrEmpty(fields))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Found invalid fields: " + fields);
            }
            else
            {
                i = 0;
                while (i < required.Length)
                {
                    if (!patchData.ContainsKey(required[i]))
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "BadRequest"), $"Found patch missing required field \"{required[i]}\"");
                    }
                    i += 1;
                }
            }
        }

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<WorkTask> Update(WorkTask request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.WorkTaskId) || !Guid.TryParse(request.WorkTaskId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work task id \"{request?.WorkTaskId}\"");
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
                IWorkTask innerWorkTask = await _workTaskFactory.Get(settings, domainId, id);
                if (innerWorkTask == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));
                IWorkTaskStatus innerWorkTaskStatus = (await _workTaskStatusFactory.GetByWorkTaskTypeId(settings, domainId, innerWorkTask.WorkTaskType.WorkTaskTypeId))
                    .FirstOrDefault(s => s.WorkTaskStatusId.Equals(Guid.Parse(request.WorkTaskStatus.WorkTaskStatusId)));
                if (innerWorkTaskStatus == null)
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Status Not Found"), "Invalid work task status. The status doesn't exist or is not valid for the task type");
                Map(request, innerWorkTask);
                innerWorkTask.WorkTaskStatus = innerWorkTaskStatus;
                ApplyContexts(request, innerWorkTask);
                await _workTaskSaver.Update(settings, innerWorkTask);
                return Map(innerWorkTask);
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

        private static void Validate(WorkTask workTask)
        {
            if (string.IsNullOrEmpty(workTask?.Title))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Missing work task title");
            if (workTask.WorkTaskStatus == null || string.IsNullOrEmpty(workTask.WorkTaskStatus.WorkTaskStatusId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Missing work task status");
        }

        private static void ApplyContexts(WorkTask workTask, IWorkTask innerWorkTask)
        {
            if (workTask.WorkTaskContexts != null)
            {
                foreach (WorkTaskContext workTaskContext in workTask.WorkTaskContexts.Where(c => c.ReferenceType.HasValue && !string.IsNullOrEmpty(c.ReferenceValue)))
                {
                    // the IWorkTask will filter out duplicates
                    innerWorkTask.AddContext((short)workTaskContext.ReferenceType.Value, workTaskContext.ReferenceValue);
                }
            }
        }

        private static void Map(Protos.WorkTask workTask, IWorkTask innerWorkTask)
        {
            innerWorkTask.AssignedDate = !string.IsNullOrEmpty(workTask.AssignedDate) ? DateTime.Parse(workTask.AssignedDate, CultureInfo.InvariantCulture) : default;
            innerWorkTask.AssignedToUserId = workTask.AssignedToUserId;
            innerWorkTask.Text = workTask.Text;
            innerWorkTask.Title = workTask.Title;
        }

        private static Protos.WorkTask Map(IWorkTask innerWorkTask)
        {
            Protos.WorkTask result = new Protos.WorkTask
            {
                AssignedDate = innerWorkTask.AssignedDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty,
                AssignedToUserId = innerWorkTask.AssignedToUserId,
                ClosedDate = innerWorkTask.ClosedDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty,
                CreateTimestamp = Timestamp.FromDateTime(innerWorkTask.CreateTimestamp),
                DomainId = innerWorkTask.DomainId.ToString("D"),
                Text = innerWorkTask.Text,
                Title = innerWorkTask.Title,
                UpdateTimestamp = Timestamp.FromDateTime(innerWorkTask.UpdateTimestamp),
                WorkTaskId = innerWorkTask.WorkTaskId.ToString("D"),
                WorkTaskStatus = innerWorkTask.WorkTaskStatus != null ? WorkTaskStatusService.Map(innerWorkTask.WorkTaskStatus) : default,
                WorkTaskType = innerWorkTask.WorkTaskType != null ? WorkTaskTypeService.Map(innerWorkTask.WorkTaskType) : default
            };
            if (innerWorkTask.WorkTaskContexts != null)
            {
                result.WorkTaskContexts.AddRange(innerWorkTask.WorkTaskContexts.Select(c => Map(c)));
            }
            return result;
        }

        private static Protos.WorkTaskContext Map(IWorkTaskContext innerWorkTaskContext)
        {
            return new Protos.WorkTaskContext
            {
                CreateTimestamp = Timestamp.FromDateTime(innerWorkTaskContext.CreateTimestamp),
                DomainId = innerWorkTaskContext.DomainId.ToString("D"),
                ReferenceType = innerWorkTaskContext.ReferenceType,
                ReferenceValue = innerWorkTaskContext.ReferenceValue,
                WorkTaskContextId = innerWorkTaskContext.WorkTaskContextId.ToString("D"),
                WorkTaskId = innerWorkTaskContext.WorkTaskId.ToString("D")
            };
        }
    }
}
