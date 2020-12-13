using BrassLoon.Interface.Log.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public class MetricService : IMetricService
    {
        private readonly IService _service;

        public MetricService(IService service)
        {
            _service = service;
        }

        public async Task<Metric> Create(ISettings settings, Metric metric)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, metric)
            .AddPath("Metric")
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            return (await _service.Send<Metric>(request)).Value;
        }

        public Task<Metric> Create(ISettings settings, Guid domainId, string eventCode, double maginitue, object data = null)
        {
            return Create(
                settings,
                new Metric
                {
                    DomainId = domainId,
                    EventCode = eventCode,
                    Magnitude = maginitue,
                    Data = data
                });
        }
    }
}
