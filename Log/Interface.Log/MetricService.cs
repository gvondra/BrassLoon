using BrassLoon.Interface.Log.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public class MetricService : IMetricService
    {
        private readonly RestUtil _restUtil;

        public MetricService(RestUtil restUtil)
        {
            _restUtil = restUtil;
        }

        public async Task<Metric> Create(ISettings settings, Metric metric)
        {
            RestRequest request = new RestRequest("Metric", Method.POST, DataFormat.Json);
            request.AddJsonBody(metric);
            return await _restUtil.Execute<Metric>(settings, request);
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
