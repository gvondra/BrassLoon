using BrassLoon.Interface.Log.Models;
using BrassLoon.RestClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public class MetricService : IMetricService
    {
        private static readonly Polly.Policy _eventCodeCache = Polly.Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(6));
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public MetricService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<Metric> Create(ISettings settings, Metric metric)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, metric)
            .AddPath("Metric")
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse<Metric> response = await Policy
                .HandleResult<IResponse<Metric>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.5) })
                .ExecuteAsync(() => _service.Send<Metric>(request))
                ;
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<Metric> Create(ISettings settings, Guid domainId, string eventCode, double magnitude, object data = null)
        {
            return Create(settings, domainId, eventCode, magnitude, data: data);
        }

        public Task<Metric> Create(ISettings settings, Guid domainId, string eventCode, double magnitude, string status = "", string requestor = "", object data = null)
        {
            return Create(settings, domainId, null, eventCode, magnitude, status, requestor, data);
        }

        public Task<Metric> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, string eventCode, double magnitude, object data = null)
        {
            return Create(settings, domainId, createTimestamp, eventCode, magnitude, data: data);
        }

        public Task<Metric> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, string eventCode, double magnitude, string status = "", string requestor = "", object data = null)
        {
            return Create(
                settings,
                new Metric
                {
                    DomainId = domainId,
                    EventCode = eventCode,
                    Magnitude = magnitude,
                    Data = data,
                    CreateTimestamp = createTimestamp,
                    Status = status,
                    Requestor = requestor
                });
        }

        public async Task<List<string>> GetEventCodes(ISettings settings, Guid domainId)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("MetricEventCode/{domainId}")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<List<string>> response = await _eventCodeCache.Execute(
                (context) => _service.Send<List<string>>(request),
                new Context(domainId.ToString("N"))
            );
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<List<Metric>> Search(ISettings settings, Guid domainId, DateTime maxTimestamp, string eventCode)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Metric")
                .AddPath(domainId.ToString("N"))
                .AddQueryParameter("maxTimestamp", maxTimestamp.ToString("o"))
                .AddQueryParameter("eventCode", eventCode ?? string.Empty)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<List<Metric>> response = await _service.Send<List<Metric>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }
    }
}
