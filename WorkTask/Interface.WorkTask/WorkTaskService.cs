using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public WorkTaskService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<ClaimWorkTaskResponse> Claim(ISettings settings, Guid domainId, Guid id, string assignToUserId, DateTime? assignedDate = null)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            if (assignToUserId == null)
                assignToUserId = string.Empty;
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put)
                .AddPath("WorkTask/{domainId}/{id}/AssignTo")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddPathParameter("id", id.ToString("N"))
                .AddQueryParameter("assignToUserId", assignToUserId)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            if (assignedDate.HasValue)
                request.AddQueryParameter("assignedDate", assignedDate.Value.Date.ToString("O"));
            return _restUtil.Send<ClaimWorkTaskResponse>(_service, request);
        }

        public Task<Models.WorkTask> Create(ISettings settings, Models.WorkTask workTask)
        {
            if (workTask == null)
                throw new ArgumentNullException(nameof(workTask));
            if (!workTask.DomainId.HasValue || workTask.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTask.DomainId)} is null");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, workTask)
                .AddPath("WorkTask")
                .AddPath(workTask.DomainId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Models.WorkTask>(_service, request);
        }

        public Task<Models.WorkTask> Get(ISettings settings, Guid domainId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("WorkTask")
                .AddPath(domainId.ToString("N"))
                .AddPath(id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Models.WorkTask>(_service, request);
        }

        public Task<List<Models.WorkTask>> GetByContext(ISettings settings, Guid domainId, short referenceType, string referenceValue)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (string.IsNullOrEmpty(referenceValue))
                throw new ArgumentNullException(nameof(referenceValue));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("WorkTask/{domain}")
                .AddPathParameter("domain", domainId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<Models.WorkTask>>(_service, request);
        }

        public Task<List<Models.WorkTask>> GetByWorkGroupId(ISettings settings, Guid domainId, Guid workGroupId, bool? includeClosed = null)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workGroupId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workGroupId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("WorkGroup/{domain}/{id}/WorkTask")
                .AddPathParameter("domain", domainId.ToString("N"))
                .AddPathParameter("id", workGroupId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            if (includeClosed != null)
                request.AddQueryParameter("includeClosed", includeClosed.Value.ToString());
            return _restUtil.Send<List<Models.WorkTask>>(_service, request);
        }

        public Task<List<Models.WorkTask>> Patch(ISettings settings, Guid domainId, IEnumerable<Dictionary<string, object>> patchData)
        {
            if (patchData == null)
                throw new ArgumentNullException(nameof(patchData));
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), new HttpMethod("PATCH"), patchData.ToArray())
                .AddPath("WorkTask/{domainId}")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<Models.WorkTask>>(_service, request);
        }

        public Task<Models.WorkTask> Update(ISettings settings, Models.WorkTask workTask)
        {
            if (workTask == null)
                throw new ArgumentNullException(nameof(workTask));
            if (!workTask.DomainId.HasValue || workTask.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTask.DomainId)} is null");
            if (!workTask.WorkTaskId.HasValue || workTask.WorkTaskId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTask.WorkTaskId)} is null");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, workTask)
                .AddPath("WorkTask")
                .AddPath(workTask.DomainId.Value.ToString("N"))
                .AddPath(workTask.WorkTaskId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Models.WorkTask>(_service, request);
        }
    }
}
