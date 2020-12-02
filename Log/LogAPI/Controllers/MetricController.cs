using LogModels = BrassLoon.Interface.Log.Models;
using Autofac;
using AutoMapper;
using BrassLoon.Interface.Account;
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

        [HttpPost()]
        [ProducesResponseType(typeof(LogModels.Metric), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Create([FromBody] LogModels.Metric metric)
        {
            IActionResult result = null;
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
                    IMetric innerMetric = factory.Create(metric.DomainId.Value, metric.EventCode);
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    mapper.Map<LogModels.Metric, IMetric>(metric, innerMetric);
                    IMetricSaver saver = scope.Resolve<IMetricSaver>();
                    await saver.Create(settings, innerMetric);
                    result = Ok(mapper.Map<LogModels.Metric>(innerMetric));
                }
            }
            return result;
        }
    }
}
