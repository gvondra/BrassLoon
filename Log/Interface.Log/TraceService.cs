using BrassLoon.Interface.Log.Models;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public class TraceService : ITraceService
    {
        private readonly RestUtil _restUtil;

        public TraceService(RestUtil restUtil)
        {
            _restUtil = restUtil;
        }

        public async Task<Trace> Create(ISettings settings, Trace trace)
        {
            RestRequest request = new RestRequest("Trace", Method.POST, DataFormat.Json);
            request.AddJsonBody(trace);
            return await _restUtil.Execute<Trace>(settings, request);
        }

        public Task<Trace> Create(ISettings settings, Guid domainId, string eventCode, string message, object data = null)
        {
            return Create(
                settings,
                new Trace
                {
                    DomainId = domainId,
                    EventCode = eventCode,
                    Message = message,
                    Data = data
                }
                );
        }
    }
}
