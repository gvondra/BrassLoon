using LogModels = BrassLoon.Interface.Log.Models;
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

namespace LogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExceptionController : LogControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly SettingsFactory _settingsFactory;
        private readonly IDomainService _domainService;
        private readonly IExceptionFactory _exceptionFactory;
        private readonly IExceptionSaver _exceptionSaver;

        public ExceptionController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IDomainService domainService,
            IExceptionFactory exceptionFactory,
            IExceptionSaver exceptionSaver)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
            _domainService = domainService;
            _exceptionFactory = exceptionFactory;
            _exceptionSaver = exceptionSaver;
        }


        [HttpGet("{domainId}")]
        [ProducesResponseType(typeof(LogModels.Exception[]), 200)]
        [Authorize()]
        public async Task<IActionResult> Search([FromRoute] Guid? domainId, [FromQuery] DateTime? maxTimestamp = null)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id prameter value");
                if (result == null && !maxTimestamp.HasValue)
                    result = BadRequest("Missing max timestamp parameter value");
                if (result == null)
                {
                    if (!(await VerifyDomainAccount(domainId.Value, _settingsFactory, _settings.Value, _domainService)))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        return Ok(  
                            await Task.WhenAll<LogModels.Exception>(
                            (await _exceptionFactory.GetTopBeforeTimestamp(settings, domainId.Value, maxTimestamp.Value))
                            .Select<IException, Task<LogModels.Exception>>(async innerException => await Map(innerException, settings, mapper))
                            ));
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
        [Authorize()]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] long? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && !id.HasValue)
                    result = BadRequest("Missing exception id parameter value");
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id prameter value");
                if (result == null)
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IException exception = await _exceptionFactory.Get(settings, id.Value);
                    if (exception != null && !exception.DomainId.Equals(domainId.Value))
                        exception = null;
                    if (result == null && !(await VerifyDomainAccount(domainId.Value, _settingsFactory, _settings.Value, _domainService)))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null && exception == null)
                        result = NotFound();
                    if (result == null && exception != null)
                    {
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(
                            await Map(exception, settings, mapper)
                            );
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

        [HttpPost()]
        [ProducesResponseType(typeof(LogModels.Exception), 200)]
        [Authorize()]
        public async Task<IActionResult> Create([FromBody] LogModels.Exception exception)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!exception.DomainId.HasValue || exception.DomainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain guid value");
                if (result == null)
                {
                    if (!(await VerifyDomainAccountWriteAccess(exception.DomainId.Value, _settingsFactory, _settings.Value, _domainService)))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (result == null)
                    {
                        CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        List<IException> allExceptions = new List<IException>();
                        IException innerException = Map(exception, exception.DomainId.Value, exception.CreateTimestamp, _exceptionFactory, mapper, allExceptions);
                        await _exceptionSaver.Create(settings, allExceptions.ToArray());
                        result = Ok(
                            await Map(innerException, settings, mapper)
                            );
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
        protected override Task<bool> VerifyDomainAccountWriteAccess(Guid domainId, SettingsFactory settingsFactory, Settings settings, IDomainService domainService)
        {
            if (!string.IsNullOrEmpty(settings.ExceptionLoggingDomainId) && Guid.Parse(settings.ExceptionLoggingDomainId).Equals(domainId))
                return Task.FromResult(true);
            else
            {
                return base.VerifyDomainAccountWriteAccess(domainId, settingsFactory, settings, domainService);
            }
        }

        [NonAction]
        private IException Map(
            LogModels.Exception exception, 
            Guid domainId, 
            DateTime? timestamp,
            IExceptionFactory exceptionFactory, 
            IMapper mapper,
            List<IException> allExceptions,
            IException parentException = null)
        {
            IException innerException = exceptionFactory.Create(domainId, timestamp, parentException);
            mapper.Map<LogModels.Exception, IException>(exception, innerException);
            allExceptions.Add(innerException);
            if (exception.InnerException != null)
                Map(exception.InnerException, domainId, timestamp, exceptionFactory, mapper, allExceptions, innerException);
            return innerException;
        }

        [NonAction]
        private async Task<LogModels.Exception> Map(IException innerException, CoreSettings settings, IMapper mapper)
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
