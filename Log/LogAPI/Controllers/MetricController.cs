using LogModels = BrassLoon.Interface.Log.Models;
using Autofac;
using AutoMapper;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using BrassLoon.Log.Framework;
using Microsoft.AspNetCore.Authorization;
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
    public class MetricController : LogControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly IContainer _container;

        public MetricController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
        }

        [HttpGet("{domainId}")]
        [ProducesResponseType(typeof(LogModels.Metric[]), 200)]
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
                        IMetricFactory metricFactory = scope.Resolve<IMetricFactory>();
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(
                            (await metricFactory.GetTopBeforeTimestamp(settings, domainId.Value, eventCode, maxTimestamp.Value))
                            .Select<IMetric, LogModels.Metric>(innerMetric => mapper.Map<LogModels.Metric>(innerMetric))
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

        [HttpGet("/api/MetricEventCode/{domainId}")]
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
                        IMetricFactory metricFactory = scope.Resolve<IMetricFactory>();
                        result = Ok(
                            await metricFactory.GetEventCodes(settings, domainId.Value)
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
        [ProducesResponseType(typeof(LogModels.Metric), 200)]
        [Authorize()]
        public async Task<IActionResult> Create([FromBody] LogModels.Metric metric)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!metric.DomainId.HasValue || metric.DomainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain guid value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    if (!(await VerifyDomainAccount(metric.DomainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                        IMetricFactory factory = scope.Resolve<IMetricFactory>();
                        IMetric innerMetric = factory.Create(metric.DomainId.Value, metric.CreateTimestamp, metric.EventCode);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        mapper.Map<LogModels.Metric, IMetric>(metric, innerMetric);
                        IMetricSaver saver = scope.Resolve<IMetricSaver>();
                        await saver.Create(settings, innerMetric);
                        result = Ok(mapper.Map<LogModels.Metric>(innerMetric));
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
