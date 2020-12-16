using LogModels = BrassLoon.Interface.Log.Models;
using Autofac;
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
        private readonly IContainer _container;

        public TraceController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
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
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                        ITraceFactory traceFactory = scope.Resolve<ITraceFactory>();
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(
                            (await traceFactory.GetTopBeforeTimestamp(settings, domainId.Value, eventCode, maxTimestamp.Value))
                            .Select<ITrace, LogModels.Trace>(innerTrace => mapper.Map<LogModels.Trace>(innerTrace))
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }
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
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                        ITraceFactory traceFactory = scope.Resolve<ITraceFactory>();
                        result = Ok(
                            await traceFactory.GetEventCodes(settings, domainId.Value)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }
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
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    if (!(await VerifyDomainAccount(trace.DomainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                        ITraceFactory factory = scope.Resolve<ITraceFactory>();
                        ITrace innerTrace = factory.Create(trace.DomainId.Value, trace.CreateTimestamp, trace.EventCode);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        mapper.Map<LogModels.Trace, ITrace>(trace, innerTrace);
                        ITraceSaver saver = scope.Resolve<ITraceSaver>();
                        await saver.Create(settings, innerTrace);
                        result = Ok(mapper.Map<LogModels.Trace>(innerTrace));
                    }
                }
            }    
            catch (Exception ex)
            {
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }    
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
