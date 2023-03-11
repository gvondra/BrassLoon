using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkTaskTypeService : IWorkTaskTypeService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public WorkTaskTypeService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<WorkTaskType> Create(ISettings settings, WorkTaskType workTaskType)
        {
            if (workTaskType == null)
                throw new ArgumentNullException(nameof(workTaskType));
            if (!workTaskType.DomainId.HasValue || workTaskType.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskType.DomainId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, workTaskType)
                .AddPath("WorkTaskType")
                .AddPath(workTaskType.DomainId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<WorkTaskType>(_service, request);
        }

        public Task<WorkTaskType> Get(ISettings settings, Guid domainId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("WorkTaskType")
                .AddPath(domainId.ToString("N"))
                .AddPath(id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<WorkTaskType>(_service, request);
        }

        public Task<List<WorkTaskType>> GetAll(ISettings settings, Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("WorkTaskType")
                .AddPath(domainId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<WorkTaskType>>(_service, request);
        }

        public Task<List<WorkTaskType>> GetByWorkGroupId(ISettings settings, Guid domainId, Guid workGroupId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workGroupId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workGroupId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("WorkGroup/{domainId}/{workGroupId}/WorkTaskType")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddPathParameter("workGroupId", workGroupId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<WorkTaskType>>(_service, request);
        }

        public Task<WorkTaskType> Update(ISettings settings, WorkTaskType workTaskType)
        {
            if (workTaskType == null)
                throw new ArgumentNullException(nameof(workTaskType));
            if (!workTaskType.DomainId.HasValue || workTaskType.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskType.DomainId));
            if (!workTaskType.WorkTaskTypeId.HasValue || workTaskType.WorkTaskTypeId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskType.WorkTaskTypeId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, workTaskType)
                .AddPath("WorkTaskType")
                .AddPath(workTaskType.DomainId.Value.ToString("N"))
                .AddPath(workTaskType.WorkTaskTypeId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<WorkTaskType>(_service, request);
        }
    }
}
