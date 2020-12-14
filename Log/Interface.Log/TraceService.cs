using BrassLoon.Interface.Log.Models;
using BrassLoon.RestClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public class TraceService : ITraceService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public TraceService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<Trace> Create(ISettings settings, Trace trace)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, trace)
            .AddPath("Trace")
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            return await _restUtil.Send<Trace>(_service, request);
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
