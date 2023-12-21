using BrassLoon.CommonAPI;
using BrassLoon.Interface.WorkTask.Protos;
using BrassLoon.WorkTask.Framework;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.WorkTask.Protos;

namespace WorkTaskRPC.Services
{
    public class WorkTaskTypeService : Protos.WorkTaskTypeService.WorkTaskTypeServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILogger<WorkTaskTypeService> _logger;
        private readonly IWorkTaskTypeFactory _workTaskTypeFactory;
        private readonly IWorkTaskTypeSaver _workTaskTypeSaver;

        public WorkTaskTypeService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            SettingsFactory settingsFactory,
            ILogger<WorkTaskTypeService> logger,
            IWorkTaskTypeFactory workTaskTypeFactory,
            IWorkTaskTypeSaver workTaskTypeSaver)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _workTaskTypeFactory = workTaskTypeFactory;
            _workTaskTypeSaver = workTaskTypeSaver;
        }

        public override async Task GetAll(GetByDomainRequest request, IServerStreamWriter<WorkTaskType> responseStream, ServerCallContext context)
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
                IEnumerable<IWorkTaskType> innerWorkTaskTypes = await _workTaskTypeFactory.GetByDomainId(settings, domainId);
                foreach (WorkTaskType workTaskType in innerWorkTaskTypes.Select(Map))
                {
                    await responseStream.WriteAsync(workTaskType);
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

        public override async Task<WorkTaskType> GetByCode(GetWorkTaskTypeByCodeRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.Code))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing work task type code value");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IWorkTaskType innerWorkTaskType = await _workTaskTypeFactory.GetByDomainIdCode(settings, domainId, request.Code);
                return innerWorkTaskType != null ? Map(innerWorkTaskType) : null;
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

        public override async Task<WorkTaskType> Get(GetWorkTaskTypeRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.WorkTaskTypeId) || !Guid.TryParse(request.WorkTaskTypeId, out id))
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
                IWorkTaskType innerWorkTaskType = await _workTaskTypeFactory.Get(settings, domainId, id);
                return innerWorkTaskType != null ? Map(innerWorkTaskType) : null;
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

        public override async Task GetByWorkGroupId(GetWorkTaskTypeByWorkGroupIdRequest request, IServerStreamWriter<WorkTaskType> responseStream, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid workGroupId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.WorkGroupId) || !Guid.TryParse(request.WorkGroupId, out workGroupId))
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
                IEnumerable<IWorkTaskType> innerWorkTaskType = await _workTaskTypeFactory.GetByWorkGroupId(settings, domainId, workGroupId);
                foreach (WorkTaskType workTaskType in innerWorkTaskType.Select(Map))
                {
                    await responseStream.WriteAsync(workTaskType);
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

        public override async Task<WorkTaskType> Create(WorkTaskType request, ServerCallContext context)
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
                IWorkTaskType innerWorkTaskType = _workTaskTypeFactory.Create(domainId, request.Code);
                Map(request, innerWorkTaskType);
                await _workTaskTypeSaver.Create(settings, innerWorkTaskType);
                return Map(innerWorkTaskType);
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

        public override async Task<WorkTaskType> Update(WorkTaskType request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.WorkTaskTypeId) || !Guid.TryParse(request.WorkTaskTypeId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid work task type id \"{request.WorkTaskTypeId}\"");
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
                IWorkTaskType innerWorkTaskType = await _workTaskTypeFactory.Get(settings, domainId, id);
                if (innerWorkTaskType == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Work Task Type Not Found"));
                Map(request, innerWorkTaskType);
                await _workTaskTypeSaver.Update(settings, innerWorkTaskType);
                return Map(innerWorkTaskType);
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

        private static void ValidateCreate(WorkTaskType workTaskType)
        {
            if (string.IsNullOrEmpty(workTaskType?.Code))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Missing type code value");
            Validate(workTaskType);
        }

        private static void Validate(WorkTaskType workTaskType)
        {
            if (string.IsNullOrEmpty(workTaskType?.Title))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), "Missing work task type title value");
        }

        private static void Map(WorkTaskType workTaskType, IWorkTaskType innerWorkTaskType)
        {
            innerWorkTaskType.Description = workTaskType.Description;
            innerWorkTaskType.PurgePeriod = workTaskType.PurgePeriod.HasValue ? (short)workTaskType.PurgePeriod.Value : default(short?);
            innerWorkTaskType.Title = workTaskType.Title;
        }

        internal static WorkTaskType Map(IWorkTaskType innerWorkTaskType)
        {
            return new WorkTaskType
            {
                Code = innerWorkTaskType.Code,
                CreateTimestamp = Timestamp.FromDateTime(innerWorkTaskType.CreateTimestamp),
                Description = innerWorkTaskType.Description,
                DomainId = innerWorkTaskType.DomainId.ToString("D"),
                PurgePeriod = innerWorkTaskType.PurgePeriod,
                Title = innerWorkTaskType.Title,
                UpdateTimestamp = Timestamp.FromDateTime(innerWorkTaskType.UpdateTimestamp),
                WorkTaskCount = innerWorkTaskType.WorkTaskCount,
                WorkTaskTypeId = innerWorkTaskType.WorkTaskTypeId.ToString("D")
            };
        }
    }
}
