using BrassLoon.Interface.Log.Models;
using BrassLoon.RestClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public class TraceService : ITraceService
    {
        private readonly IService _service;

        public TraceService(IService service)
        {
            _service = service;
        }

        public async Task<Trace> Create(ISettings settings, Trace trace)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, trace)
            .AddPath("Trace")
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            return (await _service.Send<Trace>(request)).Value;
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
