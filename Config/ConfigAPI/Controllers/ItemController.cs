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
    public class ItemController : ConfigControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly IContainer _container;

        public ItemController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
        }

        [HttpGet("{domainId}/{code}")]
        [ProducesResponseType(typeof(Item), 200)]
        [Authorize()]
        public async Task<IActionResult> GetByCode([FromRoute] Guid? domainId, [FromRoute] string code)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || Guid.Empty.Equals(domainId.Value)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && string.IsNullOrEmpty(code))
                    result = BadRequest("Missing item code parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    IItemFactory factory = scope.Resolve<IItemFactory>();
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        IItem item = await factory.GetByCode(settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (item == null)
                            result = NotFound();
                        else
                        {
                            IMapper mapper = MapperConfigurationFactory.CreateMapper();
                            result = Ok(mapper.Map<Item>(item));
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
        [Authorize()]
        public async Task<IActionResult> GetDataByCode([FromRoute] Guid? domainId, [FromRoute] string code)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || Guid.Empty.Equals(domainId.Value)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && string.IsNullOrEmpty(code))
                    result = BadRequest("Missing item code parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    IItemFactory factory = scope.Resolve<IItemFactory>();
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        IItem item = await factory.GetByCode(settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (item == null)
                            result = NotFound();
                        else
                        {
                            result = Ok(item.Data);
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
        [ProducesResponseType(typeof(Item), 200)]
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
                    IItemFactory factory = scope.Resolve<IItemFactory>();
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
        [ProducesResponseType(typeof(Item), 200)]
        [Authorize()]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] string code, [FromBody] dynamic itemData)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || Guid.Empty.Equals(domainId.Value)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && string.IsNullOrEmpty(code))
                    result = BadRequest("Missing item code parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateCore(_settings.Value);
                    IItemFactory factory = scope.Resolve<IItemFactory>();
                    IItem innerItem = null;
                    Func<CoreSettings, IItemSaver, IItem, Task> save = (sttngs, svr, lkup) => svr.Update(sttngs, lkup);
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                        innerItem = await factory.GetByCode(settings, domainId.Value, code);
                    if (result == null && innerItem == null)
                    {
                        innerItem = factory.Create(domainId.Value, code);
                        save = (sttngs, svr, lkup) => svr.Create(sttngs, lkup);
                    }
                    if (result == null && innerItem != null)
                    {
                        innerItem.Data = itemData;
                        await save(settings, scope.Resolve<IItemSaver>(), innerItem);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(
                            mapper.Map<Item>(innerItem)
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
                    result = BadRequest("Missing item code parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    if (!(await VerifyDomainAccount(domainId.Value, settingsFactory, _settings.Value, scope.Resolve<IDomainService>())))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    else
                    {
                        IItemSaver saver = scope.Resolve<IItemSaver>();
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
