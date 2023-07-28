using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.RestClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkTaskTypeService : IWorkTaskTypeService
    {
        private static AsyncPolicy _getCache = CreateCachePolicy();
        private static AsyncPolicy _getByCodeCache = CreateCachePolicy();
        private static AsyncPolicy _getByWorkGroupIdCache = CreateCachePolicy();
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public WorkTaskTypeService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<WorkTaskType> Create(ISettings settings, WorkTaskType workTaskType)
        {
            if (workTaskType == null)
                throw new ArgumentNullException(nameof(workTaskType));
            if (!workTaskType.DomainId.HasValue || workTaskType.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskType.DomainId)} is null");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, workTaskType)
                .AddPath("WorkTaskType")
                .AddPath(workTaskType.DomainId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            WorkTaskType result = await _restUtil.Send<WorkTaskType>(_service, request);
            ResetAllCaches();
            return result;
        }

        public Task<WorkTaskType> Get(ISettings settings, Guid domainId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            return _getCache.ExecuteAsync(
                (context) => GetUncached(settings, domainId, id),
                new Context($"{domainId:N}|{id:N}"));
        }

        private Task<WorkTaskType> GetUncached(ISettings settings, Guid domainId, Guid id)
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

        public Task<WorkTaskType> GetByCode(ISettings settings, Guid domainId, string code)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));
            return _getByCodeCache.ExecuteAsync(
                (context) => GetByCodeUncached(settings, domainId, code),
                new Context($"{domainId:N}|{code}"));
        }

        private Task<WorkTaskType> GetByCodeUncached(ISettings settings, Guid domainId, string code)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("WorkTaskType")
                .AddPath(domainId.ToString("N"))
                .AddQueryParameter("code", code)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<WorkTaskType>(_service, request);
        }

        public Task<List<WorkTaskType>> GetByWorkGroupId(ISettings settings, Guid domainId, Guid workGroupId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workGroupId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workGroupId));
            return _getByWorkGroupIdCache.ExecuteAsync(
                (context) => GetByWorkGroupIdUncached(settings, domainId, workGroupId),
                new Context($"{domainId:N}|{workGroupId:N}"));
        }

        private Task<List<WorkTaskType>> GetByWorkGroupIdUncached(ISettings settings, Guid domainId, Guid workGroupId)
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

        public async Task<WorkTaskType> Update(ISettings settings, WorkTaskType workTaskType)
        {
            if (workTaskType == null)
                throw new ArgumentNullException(nameof(workTaskType));
            if (!workTaskType.DomainId.HasValue || workTaskType.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskType.DomainId)} is null");
            if (!workTaskType.WorkTaskTypeId.HasValue || workTaskType.WorkTaskTypeId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskType.WorkTaskTypeId)} is null");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, workTaskType)
                .AddPath("WorkTaskType")
                .AddPath(workTaskType.DomainId.Value.ToString("N"))
                .AddPath(workTaskType.WorkTaskTypeId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            WorkTaskType result = await _restUtil.Send<WorkTaskType>(_service, request);
            ResetAllCaches();
            return result;
        }

        private static AsyncPolicy CreateCachePolicy() => Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromMinutes(5)));

        private static void ResetAllCaches()
        {
            _getCache = CreateCachePolicy();
            _getByCodeCache = CreateCachePolicy();
            _getByWorkGroupIdCache = CreateCachePolicy();
        }
    }
}
