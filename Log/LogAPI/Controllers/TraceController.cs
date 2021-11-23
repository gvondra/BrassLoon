using LogModels = BrassLoon.Interface.Log.Models;
using AutoMapper;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using BrassLoon.Log.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraceController : LogControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly SettingsFactory _settingsFactory;
        private readonly IDomainService _domainService;
        private readonly Lazy<IExceptionService> _exceptionService;
        private readonly ITraceFactory _traceFactory;
        private readonly ITraceSaver _traceSaver;

        public TraceController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IDomainService domainService,
            Lazy<IExceptionService> exceptionService,
            ITraceFactory traceFactory,
            ITraceSaver traceSaver)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
            _domainService = domainService;
            _exceptionService = exceptionService;
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
                    if (!(await VerifyDomainAccount(domainId.Value, _settingsFactory, _settings.Value, _domainService)))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(
                            (await _traceFactory.GetTopBeforeTimestamp(settings, domainId.Value, eventCode, maxTimestamp.Value))
                            .Select<ITrace, LogModels.Trace>(innerTrace => mapper.Map<LogModels.Trace>(innerTrace))
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
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
                    if (!(await VerifyDomainAccount(domainId.Value, _settingsFactory, _settings.Value, _domainService)))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                        result = Ok(
                            await _traceFactory.GetEventCodes(settings, domainId.Value)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
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
                    if (!(await VerifyDomainAccountWriteAccess(trace.DomainId.Value, _settingsFactory, _settings.Value, _domainService)))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                        ITrace innerTrace = _traceFactory.Create(trace.DomainId.Value, trace.CreateTimestamp, trace.EventCode);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        mapper.Map<LogModels.Trace, ITrace>(trace, innerTrace);
                        await _traceSaver.Create(settings, innerTrace);
                        result = Ok(mapper.Map<LogModels.Trace>(innerTrace));
                    }
                }
            }    
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
