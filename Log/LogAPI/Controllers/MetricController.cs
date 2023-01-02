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
    public class MetricController : LogControllerBase
    {
        private readonly IMetricFactory _metricFactory;
        private readonly IMetricSaver _metricSaver;
        private readonly IEventIdFactory _eventIdFactory;

        public MetricController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Log.IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IMetricFactory metricFactory,
            IMetricSaver metricSaver,
            IEventIdFactory eventIdFactory)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _metricFactory = metricFactory;
            _metricSaver = metricSaver;
            _eventIdFactory = eventIdFactory;
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
                        IEventId eventId = await GetInnerEventId(settings, metric.DomainId.Value, metric.EventId);
                        IMetric innerMetric = _metricFactory.Create(metric.DomainId.Value, metric.CreateTimestamp, metric.EventCode, eventId);
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

        [HttpPost("/api/MetricBatch/{domainId}")]
        [Authorize()]
        public async Task<IActionResult> CreateBatch([FromRoute] Guid? domainId, [FromBody] List<LogModels.Metric> metrics)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain guid value");
                if (result == null && metrics == null)
                    result = BadRequest("Missing metric list message body");
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
                        List<IMetric> innerMetrics = new List<IMetric>(metrics.Count);
                        foreach (LogModels.Metric metric in metrics)
                        {
                            IEventId eventId = await GetInnerEventId(settings,domainId.Value, metric.EventId);
                            IMetric innerMetric = _metricFactory.Create(domainId.Value, metric.CreateTimestamp, metric.EventCode, eventId);
                            mapper.Map<LogModels.Metric, IMetric>(metric, innerMetric);
                            innerMetrics.Add(innerMetric);
                        }
                        await _metricSaver.Create(settings, innerMetrics.ToArray());
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
