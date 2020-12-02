using LogModels = BrassLoon.Interface.Log.Models;
using Autofac;
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
        private readonly IContainer _container;

        public ExceptionController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(LogModels.Exception), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Create([FromBody] LogModels.Exception exception)
        {
            IActionResult result = null;
            if (result == null && (!exception.DomainId.HasValue || exception.DomainId.Value.Equals(Guid.Empty)))
                result = BadRequest("Missing domain guid value");
            if (result == null)
            {
                using ILifetimeScope scope = _container.BeginLifetimeScope();
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                if (!(await VerifyDomainAccount(exception.DomainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                if (result == null)
                {
                    CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                    IExceptionFactory factory = scope.Resolve<IExceptionFactory>();
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    List<IException> allExceptions = new List<IException>();
                    IException innerException = Map(exception, exception.DomainId.Value, factory, mapper, allExceptions);
                    IExceptionSaver saver = scope.Resolve<IExceptionSaver>();
                    await saver.Create(settings, allExceptions.ToArray());
                    result = Ok(
                        await Map(innerException, settings, mapper)
                        );
                }
            }
            return result;
        }

        [NonAction]
        private IException Map(
            LogModels.Exception exception, 
            Guid domainId, 
            IExceptionFactory exceptionFactory, 
            IMapper mapper,
            List<IException> allExceptions,
            IException parentException = null)
        {
            IException innerException = exceptionFactory.Create(domainId, parentException);
            mapper.Map<LogModels.Exception, IException>(exception, innerException);
            allExceptions.Add(innerException);
            if (exception.InnerException != null)
                Map(exception.InnerException, domainId, exceptionFactory, mapper, allExceptions, innerException);
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
