using LogModels = BrassLoon.Interface.Log.Models;
using Autofac;
using AutoMapper;
using BrassLoon.Log.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraceController : ControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly IContainer _container;

        public TraceController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(LogModels.Trace), 200)]
        public async Task<IActionResult> Create([FromBody] LogModels.Trace trace)
        {
            IActionResult result = null;
            if (!trace.DomainId.HasValue || trace.DomainId.Value.Equals(Guid.Empty))
                result = BadRequest("Missing domain guid value");            
            if (result == null)
            {
                using ILifetimeScope scope = _container.BeginLifetimeScope();
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                ITraceFactory factory = scope.Resolve<ITraceFactory>();
                ITrace innerTrace = factory.Create(trace.DomainId.Value, trace.EventCode);
                IMapper mapper = MapperConfigurationFactory.CreateMapper();
                mapper.Map<LogModels.Trace, ITrace>(trace, innerTrace);
                ITraceSaver saver = scope.Resolve<ITraceSaver>();
                await saver.Create(settings, innerTrace);
                result = Ok(mapper.Map<LogModels.Trace>(innerTrace));
            }
            return result;
        }
    }
}
