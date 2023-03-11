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
    public class WorkTaskCommentService : IWorkTaskCommentService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public WorkTaskCommentService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<List<Comment>> Create(ISettings settings, Guid domainId, Guid workTaskId, params Comment[] comments)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, comments)
                .AddPath("WorkTask/{domainId}/{workTaskId}/WorkGroup")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddPathParameter("workTaskId", workTaskId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<Comment>>(_service, request);
        }

        public Task<List<Comment>> Create(ISettings settings, Guid domainId, Guid workTaskId, params string[] comments)
        => Create(settings, domainId, workTaskId, comments.Select<string, Comment>(c => new Comment { DomainId = domainId, Text = c }).ToArray());

        public Task<List<Comment>> GetAll(ISettings settings, Guid domainId, Guid workTaskId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("WorkTask/{domainId}/{workTaskId}/WorkGroup")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddPathParameter("workTaskId", workTaskId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<Comment>>(_service, request);
        }
    }
}
