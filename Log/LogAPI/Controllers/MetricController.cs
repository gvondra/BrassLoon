using AutoMapper;
using BrassLoon.Interface.Account;
using BrassLoon.Log.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Log = BrassLoon.Interface.Log;
using LogModels = BrassLoon.Interface.Log.Models;

namespace LogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricController : LogControllerBase
    {
        private readonly IMetricFactory _metricFactory;
        private readonly IMetricSaver _metricSaver;

        public MetricController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Log.IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IMetricFactory metricFactory,
            IMetricSaver metricSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _metricFactory = metricFactory;
            _metricSaver = metricSaver;
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
                    if (!(await VerifyDomainAccount(domainId.Value)))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        CoreSettings settings = CreateCoreSettings();
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            (await _metricFactory.GetTopBeforeTimestamp(settings, domainId.Value, eventCode, maxTimestamp.Value))
                            .Select<IMetric, LogModels.Metric>(innerMetric => mapper.Map<LogModels.Metric>(innerMetric))
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
                    if (!(await VerifyDomainAccount(domainId.Value)))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        CoreSettings settings = CreateCoreSettings();
                        result = Ok(
                            await _metricFactory.GetEventCodes(settings, domainId.Value)
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
                    if (!(await VerifyDomainAccountWriteAccess(metric.DomainId.Value, _settings.Value, _domainService)))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        CoreSettings settings = CreateCoreSettings();
                        IMetric innerMetric = _metricFactory.Create(metric.DomainId.Value, metric.CreateTimestamp, metric.EventCode, metric.Status, metric.Requestor);
                        IMapper mapper = CreateMapper();
                        mapper.Map<LogModels.Metric, IMetric>(metric, innerMetric);
                        await _metricSaver.Create(settings, innerMetric);
                        result = Ok(mapper.Map<LogModels.Metric>(innerMetric));
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
