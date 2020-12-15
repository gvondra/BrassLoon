using BrassLoon.Interface.Log.Models;
using BrassLoon.RestClient;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public class MetricService : IMetricService
    {
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

        public Task<Metric> Create(ISettings settings, Guid domainId, string eventCode, double maginitue, object data = null)
        {
            return Create(settings, domainId, null, eventCode, maginitue, data);
        }

        public Task<Metric> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, string eventCode, double maginitue, object data = null)
        {
            return Create(
                settings,
                new Metric
                {
                    DomainId = domainId,
                    EventCode = eventCode,
                    Magnitude = maginitue,
                    Data = data,
                    CreateTimestamp = createTimestamp
                });
        }
    }
}
