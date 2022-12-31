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

        public TraceController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Log.IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            ITraceFactory traceFactory,
            ITraceSaver traceSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _traceFactory = traceFactory;
            _traceSaver = traceSaver;
        }

        [HttpGet("{domainId}")]
        [ProducesResponseType(typeof(LogModels.Trace[]), 200)]
        [Authorize()]
        public async Task<IActionResult> Search([FromRoute] Guid? domainId, [FromQuery] DateTime? maxTimestamp = null, [FromQuery] string eventCode = null)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !maxTimestamp.HasValue)
                    result = BadRequest("Missing max timestamp parameter value");
                if (result == null && string.IsNullOrEmpty(eventCode))
                    result = BadRequest("Missing event code parameter value");
                if (result == null)
                {
                    if (!(await VerifyDomainAccount(domainId.Value)))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        CoreSettings settings = CreateCoreSettings();
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            (await _traceFactory.GetTopBeforeTimestamp(settings, domainId.Value, eventCode, maxTimestamp.Value))
                            .Select<ITrace, LogModels.Trace>(innerTrace => mapper.Map<LogModels.Trace>(innerTrace))
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
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null)
                {
                    if (!(await VerifyDomainAccount(domainId.Value)))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
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

        [HttpPost()]
        [ProducesResponseType(typeof(LogModels.Trace), 200)]
        [Authorize()]
        public async Task<IActionResult> Create([FromBody] LogModels.Trace trace)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!trace.DomainId.HasValue || trace.DomainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain guid value");
                if (result == null)
                {
                    if (!(await VerifyDomainAccountWriteAccess(trace.DomainId.Value, _settings.Value, _domainService)))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        CoreSettings settings = CreateCoreSettings();
                        ITrace innerTrace = _traceFactory.Create(trace.DomainId.Value, trace.CreateTimestamp, trace.EventCode);
                        IMapper mapper = CreateMapper();
                        mapper.Map<LogModels.Trace, ITrace>(trace, innerTrace);
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
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain guid value");
                if (result == null && traces == null)
                    result = BadRequest("Missing trace list message body");
                if (result == null)
                {
                    if (!(await VerifyDomainAccountWriteAccess(domainId.Value, _settings.Value, _domainService)))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        CoreSettings settings = CreateCoreSettings();
                        IMapper mapper = CreateMapper();
                        List<Task> createTasks = new List<Task>();
                        foreach (LogModels.Trace trace in traces)
                        {
                            ITrace innerTrace = _traceFactory.Create(domainId.Value, trace.CreateTimestamp, trace.EventCode);
                            mapper.Map<LogModels.Trace, ITrace>(trace, innerTrace);
                            createTasks.Add(_traceSaver.Create(settings, innerTrace));
                        }
                        await Task.WhenAll(createTasks);
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
