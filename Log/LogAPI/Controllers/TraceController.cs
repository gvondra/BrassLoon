using AutoMapper;
using BrassLoon.Interface.Account;
using BrassLoon.Log.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Log = BrassLoon.Interface.Log;
using LogModels = BrassLoon.Interface.Log.Models;

namespace LogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraceController : LogControllerBase
    {
        private readonly ITraceFactory _traceFactory;
        private readonly ITraceSaver _traceSaver;
        private readonly IEventIdFactory _eventIdFactory;

        public TraceController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Log.IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            ITraceFactory traceFactory,
            ITraceSaver traceSaver,
            IEventIdFactory eventIdFactory)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _traceFactory = traceFactory;
            _traceSaver = traceSaver;
            _eventIdFactory = eventIdFactory;
        }

        [HttpGet("{domainId}")]
        [ProducesResponseType(typeof(LogModels.Trace[]), 200)]
        [Authorize()]
        public async Task<IActionResult> Search([FromRoute] Guid? domainId, [FromQuery] DateTime? maxTimestamp = null, [FromQuery] string eventCode = null)
        {
            IActionResult result = null;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (!maxTimestamp.HasValue)
                {
                    result = BadRequest("Missing max timestamp parameter value");
                }
                else if (string.IsNullOrEmpty(eventCode))
                {
                    result = BadRequest("Missing event code parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        CoreSettings settings = CreateCoreSettings();
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            (await _traceFactory.GetTopBeforeTimestamp(settings, domainId.Value, eventCode, maxTimestamp.Value))
                            .Select(mapper.Map<LogModels.Trace>)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("/api/TraceEventCode/{domainId}")]
        [ProducesResponseType(typeof(string[]), 200)]
        [ResponseCache(Duration = 150, Location = ResponseCacheLocation.Client)]
        [Authorize()]
        public async Task<IActionResult> GetEventCode([FromRoute] Guid? domainId)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        CoreSettings settings = CreateCoreSettings();
                        result = Ok(
                            await _traceFactory.GetEventCodes(settings, domainId.Value)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        private async Task<IEventId> GetInnerEventId(CoreSettings settings, Guid domainId, LogModels.EventId? eventId)
        {
            IEventId innerEventId = null;
            if (eventId.HasValue && (eventId.Value.Id != 0 || !string.IsNullOrEmpty(eventId.Value.Name)))
            {
                innerEventId = (await _eventIdFactory.GetByDomainId(settings, domainId))
                                .FirstOrDefault(i => i.Id == eventId.Value.Id && string.Equals(i.Name, eventId.Value.Name, StringComparison.OrdinalIgnoreCase))
                                ?? _eventIdFactory.Create(domainId, eventId.Value.Id, eventId.Value.Name);
            }
            return innerEventId;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(LogModels.Trace), 200)]
        [Authorize()]
        public async Task<IActionResult> Create([FromBody] LogModels.Trace trace)
        {
            IActionResult result = null;
            try
            {
                if (!trace.DomainId.HasValue || trace.DomainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain guid value");
                }
                else
                {
                    if (!await VerifyDomainAccountWriteAccess(trace.DomainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        CoreSettings settings = CreateCoreSettings();
                        IEventId innerEventId = await GetInnerEventId(settings, trace.DomainId.Value, trace.EventId);
                        ITrace innerTrace = _traceFactory.Create(trace.DomainId.Value, trace.CreateTimestamp, trace.EventCode, innerEventId);
                        IMapper mapper = CreateMapper();
                        _ = mapper.Map(trace, innerTrace);
                        await _traceSaver.Create(settings, innerTrace);
                        result = Ok(mapper.Map<LogModels.Trace>(innerTrace));
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPost("/api/TraceBatch/{domainId}")]
        [Authorize()]
        public async Task<IActionResult> CreateBatch([FromRoute] Guid? domainId, [FromBody] List<LogModels.Trace> traces)
        {
            IActionResult result = null;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain guid value");
                }
                else if (traces == null)
                {
                    result = BadRequest("Missing trace list message body");
                }
                else
                {
                    if (!await VerifyDomainAccountWriteAccess(domainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        CoreSettings settings = CreateCoreSettings();
                        IMapper mapper = CreateMapper();
                        List<ITrace> innerTraces = new List<ITrace>(traces.Count);
                        foreach (LogModels.Trace trace in traces)
                        {
                            IEventId innerEventId = await GetInnerEventId(settings, domainId.Value, trace.EventId);
                            ITrace innerTrace = _traceFactory.Create(domainId.Value, trace.CreateTimestamp, trace.EventCode, innerEventId);
                            _ = mapper.Map(trace, innerTrace);
                            innerTraces.Add(innerTrace);
                        }
                        await _traceSaver.Create(settings, innerTraces.ToArray());
                        result = Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
