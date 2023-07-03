using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkTaskStatusService : IWorkTaskStatusService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public WorkTaskStatusService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<WorkTaskStatus> Create(ISettings settings, WorkTaskStatus workTaskStatus)
        {
            if (workTaskStatus == null)
                throw new ArgumentNullException(nameof(workTaskStatus));
            if (!workTaskStatus.DomainId.HasValue || workTaskStatus.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskStatus.DomainId)} is null");
            if (!workTaskStatus.WorkTaskTypeId.HasValue || workTaskStatus.WorkTaskTypeId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskStatus.WorkTaskTypeId)} is null");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, workTaskStatus)
                .AddPath("WorkTaskType/{domainId}/{workTaskTypeId}/Status")
                .AddPathParameter("domainId", workTaskStatus.DomainId.Value.ToString("N"))
                .AddPathParameter("workTaskTypeId", workTaskStatus.WorkTaskTypeId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<WorkTaskStatus>(_service, request);
        }

        public async Task Delete(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskTypeId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskTypeId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Delete)
                .AddPath("WorkTaskType/{domainId}/{workTaskTypeId}/Status/{id}")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddPathParameter("workTaskTypeId", workTaskTypeId.ToString("N"))
                .AddPathParameter("id", id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse response = await _service.Send(request);
            _restUtil.CheckSuccess(response);
        }

        public Task<WorkTaskStatus> Get(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskTypeId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskTypeId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("WorkTaskType/{domainId}/{workTaskTypeId}/Status/{id}")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddPathParameter("workTaskTypeId", workTaskTypeId.ToString("N"))
                .AddPathParameter("id", id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<WorkTaskStatus>(_service, request);
        }

        public Task<List<WorkTaskStatus>> GetAll(ISettings settings, Guid domainId, Guid workTaskTypeId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskTypeId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskTypeId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("WorkTaskType/{domainId}/{workTaskTypeId}/Status")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddPathParameter("workTaskTypeId", workTaskTypeId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<WorkTaskStatus>>(_service, request);
        }

        public Task<WorkTaskStatus> Update(ISettings settings, WorkTaskStatus workTaskStatus)
        {
            if (workTaskStatus == null)
                throw new ArgumentNullException(nameof(workTaskStatus));
            if (!workTaskStatus.DomainId.HasValue || workTaskStatus.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskStatus.DomainId)} is null");
            if (!workTaskStatus.WorkTaskTypeId.HasValue || workTaskStatus.WorkTaskTypeId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskStatus.WorkTaskTypeId)} is null");
            if (!workTaskStatus.WorkTaskStatusId.HasValue || workTaskStatus.WorkTaskStatusId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskStatus.WorkTaskStatusId)} is null");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, workTaskStatus)
                .AddPath("WorkTaskType/{domainId}/{workTaskTypeId}/Status/{id}")
                .AddPathParameter("domainId", workTaskStatus.DomainId.Value.ToString("N"))
                .AddPathParameter("workTaskTypeId", workTaskStatus.WorkTaskTypeId.Value.ToString("N"))
                .AddPathParameter("id", workTaskStatus.WorkTaskStatusId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<WorkTaskStatus>(_service, request);
        }
    }
}
