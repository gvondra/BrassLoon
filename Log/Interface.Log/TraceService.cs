using BrassLoon.Interface.Log.Models;
using BrassLoon.RestClient;
using Polly;
using System;
using System.Collections.Generic;
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
            IResponse<Trace> response = await Policy
                .HandleResult<IResponse<Trace>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.5) })
                .ExecuteAsync(() => _service.Send<Trace>(request))
                ;
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<Trace> Create(ISettings settings, Guid domainId, string eventCode, string message, object data = null)
        {
            return Create(settings, domainId, null, eventCode, message, data);
        }

        public Task<Trace> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, string eventCode, string message, object data = null)
        {
            return Create(
                settings,
                new Trace
                {
                    DomainId = domainId,
                    EventCode = eventCode,
                    Message = message,
                    Data = data,
                    CreateTimestamp = createTimestamp
                }
                );
        }

        public async Task Create(ISettings settings, Guid domainId, List<Trace> traces)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, traces)
            .AddPath("TraceBatch/{domainId}")
            .AddPathParameter("domainId", domainId.ToString("D"))
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse response = await Policy
                .HandleResult<IResponse<Trace>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(0.2) })
                .ExecuteAsync(() => _service.Send<Trace>(request))
                ;
            _restUtil.CheckSuccess(response);
        }
    }
}
