using Autofac;
using AutoMapper;
using BrassLoon.Config.Framework;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Config.Models;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ConfigControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly IContainer _container;

        public LookupController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
        }

        [HttpGet("{domainId}/{code}")]
        [ProducesResponseType(typeof(Lookup), 200)]
        [Authorize()]
        public async Task<IActionResult> GetByCode([FromRoute] Guid? domainId, [FromRoute] string code)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || Guid.Empty.Equals(domainId.Value)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && string.IsNullOrEmpty(code))
                    result = BadRequest("Missing lookup code parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    ILookupFactory factory = scope.Resolve<ILookupFactory>();
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        ILookup lookup = await factory.GetByCode(settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (lookup == null)
                            result = NotFound();
                        else
                        {
                            IMapper mapper = MapperConfigurationFactory.CreateMapper();
                            result = Ok(mapper.Map<Lookup>(lookup));
                        }
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

        [HttpGet("{domainId}/{code}/Data")]
        [ProducesResponseType(typeof(Dictionary<string, string>), 200)]
        [Authorize()]
        public async Task<IActionResult> GetDataByCode([FromRoute] Guid? domainId, [FromRoute] string code)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || Guid.Empty.Equals(domainId.Value)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && string.IsNullOrEmpty(code))
                    result = BadRequest("Missing lookup code parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    ILookupFactory factory = scope.Resolve<ILookupFactory>();
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        ILookup lookup = await factory.GetByCode(settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (lookup == null)
                            result = NotFound();
                        else
                        {
                            IMapper mapper = MapperConfigurationFactory.CreateMapper();
                            result = Ok(mapper.Map<Dictionary<string, string>>(lookup.Data));
                        }
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

        [HttpGet("{domainId}/{code}/History")]
        [ProducesResponseType(typeof(LookupHistory[]), 200)]
        [Authorize()]
        public async Task<IActionResult> GetHistoryByCode([FromRoute] Guid? domainId, [FromRoute] string code)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || Guid.Empty.Equals(domainId.Value)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && string.IsNullOrEmpty(code))
                    result = BadRequest("Missing lookup code parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    ILookupFactory factory = scope.Resolve<ILookupFactory>();
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        ILookup lookup = await factory.GetByCode(settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (lookup == null)
                            result = NotFound();
                        else
                        {
                            IMapper mapper = MapperConfigurationFactory.CreateMapper();
                            result = Ok(
                                (await lookup.GetHistory(settingsFactory.CreateCore(_settings.Value)))
                                .Select<ILookupHistory, LookupHistory>(hist => mapper.Map<LookupHistory>(hist))
                                );
                        }
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

        [HttpGet("/api/[controller]Code/{domainId}")]
        [ProducesResponseType(typeof(Lookup), 200)]
        [Authorize()]
        public async Task<IActionResult> GetCodes([FromRoute] Guid? domainId)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || Guid.Empty.Equals(domainId.Value)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    ILookupFactory factory = scope.Resolve<ILookupFactory>();
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        result = Ok(
                            await factory.GetCodes(settingsFactory.CreateCore(_settings.Value), domainId.Value)
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

        [HttpPut("{domainId}/{code}/Data")]
        [ProducesResponseType(typeof(Lookup), 200)]
        [Authorize()]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] string code, [FromBody] Dictionary<string, string> lookupData)
        {
            IActionResult result = null;
            try 
            {
                if (result == null && (!domainId.HasValue || Guid.Empty.Equals(domainId.Value)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && string.IsNullOrEmpty(code))
                    result = BadRequest("Missing lookup code parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                    ILookupFactory factory = scope.Resolve<ILookupFactory>();
                    ILookup innerLookup = null;
                    Func<CoreSettings, ILookupSaver, ILookup, Task> save = (sttngs, svr, lkup) => svr.Update(sttngs, lkup);
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                        innerLookup = await factory.GetByCode(settings, domainId.Value, code);
                    if (result == null && innerLookup == null)
                    {
                        innerLookup = factory.Create(domainId.Value, code);
                        save = (sttngs, svr, lkup) => svr.Create(sttngs, lkup);
                    }
                    if (result == null && innerLookup != null)
                    {
                        innerLookup.Data = lookupData;
                        await save(settings, scope.Resolve<ILookupSaver>(), innerLookup);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(
                            mapper.Map<Lookup>(innerLookup)
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

        [HttpDelete("{domainId}/{code}")]
        [Authorize()]
        public async Task<IActionResult> Delete([FromRoute] Guid? domainId, [FromRoute] string code)
        {
            IActionResult result = null;
            try 
            {
                if (result == null && (!domainId.HasValue || Guid.Empty.Equals(domainId.Value)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && string.IsNullOrEmpty(code))
                    result = BadRequest("Missing lookup code parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        ILookupSaver saver = scope.Resolve<ILookupSaver>();
                        await saver.DeleteByCode(settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        result = Ok();
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