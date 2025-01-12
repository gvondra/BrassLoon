using AutoMapper;
using BrassLoon.CommonAPI;
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
    public class ExceptionController : LogControllerBase
    {
        private readonly IExceptionFactory _exceptionFactory;
        private readonly IExceptionSaver _exceptionSaver;
        private readonly IEventIdFactory _eventIdFactory;

        public ExceptionController(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Log.IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IExceptionFactory exceptionFactory,
            IExceptionSaver exceptionSaver,
            IEventIdFactory eventIdFactory)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _exceptionFactory = exceptionFactory;
            _exceptionSaver = exceptionSaver;
            _eventIdFactory = eventIdFactory;
        }

        [HttpGet("{domainId}")]
        [ProducesResponseType(typeof(LogModels.Exception[]), 200)]
        [Authorize]
        public async Task<IActionResult> Search([FromRoute] Guid? domainId, [FromQuery] DateTime? maxTimestamp = null)
        {
            IActionResult result = null;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id prameter value");
                }
                else if (!maxTimestamp.HasValue)
                {
                    result = BadRequest("Missing max timestamp parameter value");
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
                        return Ok(
                            await Task.WhenAll(
                            (await _exceptionFactory.GetTopBeforeTimestamp(settings, domainId.Value, maxTimestamp.Value))
                            .Select(async innerException => await Map(innerException, settings, mapper))));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{domainId}/{id}")]
        [ProducesResponseType(typeof(LogModels.Exception), 200)]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (!id.HasValue)
                {
                    result = BadRequest("Missing exception id parameter value");
                }
                else if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id prameter value");
                }
                else
                {
                    CoreSettings settings = CreateCoreSettings();
                    IException exception = await _exceptionFactory.Get(settings, id.Value);
                    if (exception != null && !exception.DomainId.Equals(domainId.Value))
                        exception = null;
                    if (!await VerifyDomainAccount(domainId.Value))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null && exception == null)
                        result = NotFound();
                    if (result == null && exception != null)
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            await Map(exception, settings, mapper));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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

        [HttpPost]
        [ProducesResponseType(typeof(LogModels.Exception), 200)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] LogModels.Exception exception)
        {
            IActionResult result = null;
            try
            {
                if (!exception.DomainId.HasValue || exception.DomainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain guid value");
                }
                else
                {
                    if (!await VerifyDomainAccountWriteAccess(exception.DomainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        CoreSettings settings = CreateCoreSettings();
                        IMapper mapper = CreateMapper();
                        List<IException> allExceptions = new List<IException>();
                        IException innerException = await Map(settings, exception, exception.DomainId.Value, exception.CreateTimestamp, _exceptionFactory, mapper, allExceptions);
                        await _exceptionSaver.Create(settings, allExceptions.ToArray());
                        result = Ok(
                            await Map(innerException, settings, mapper));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        protected override Task<bool> VerifyDomainAccountWriteAccess(Guid domainId, CommonApiSettings settings, IDomainService domainService)
        {
            if (!string.IsNullOrEmpty(settings.ExceptionLoggingDomainId) && Guid.Parse(settings.ExceptionLoggingDomainId).Equals(domainId))
            {
                return Task.FromResult(true);
            }
            else
            {
                return base.VerifyDomainAccountWriteAccess(domainId, settings, domainService);
            }
        }

        [NonAction]
        private async Task<IException> Map(
            CoreSettings settings,
            LogModels.Exception exception,
            Guid domainId,
            DateTime? timestamp,
            IExceptionFactory exceptionFactory,
            IMapper mapper,
            List<IException> allExceptions,
            IException parentException = null)
        {
            IEventId innerEventId = await GetInnerEventId(settings, domainId, exception.EventId);
            IException innerException = exceptionFactory.Create(domainId, timestamp, parentException, innerEventId);
            _ = mapper.Map(exception, innerException);
            allExceptions.Add(innerException);
            if (exception.InnerException != null)
                _ = await Map(settings, exception.InnerException, domainId, timestamp, exceptionFactory, mapper, allExceptions, innerException);
            return innerException;
        }

        [NonAction]
        private static async Task<LogModels.Exception> Map(IException innerException, CoreSettings settings, IMapper mapper)
        {
            LogModels.Exception exception = mapper.Map<LogModels.Exception>(innerException);
            IException innerException2 = await innerException.GetInnerException(settings);
            if (innerException2 != null)
            {
                exception.InnerException = await Map(innerException2, settings, mapper);
            }
            return exception;
        }
    }
}
