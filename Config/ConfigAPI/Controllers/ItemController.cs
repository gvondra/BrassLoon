using AutoMapper;
using BrassLoon.CommonAPI;
using BrassLoon.Config.Framework;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Config.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ItemController> _logger;
        private readonly IDomainService _domainService;
        private readonly IItemFactory _itemFactory;
        private readonly IItemSaver _itemSaver;

        public ItemController(
            IOptions<Settings> settings,
            ISettingsFactory settingsFactory,
            ILogger<ItemController> logger,
            IDomainService domainService,
            IItemFactory itemFactory,
            IItemSaver itemSaver)
            : base(settings, settingsFactory)
        {
            _logger = logger;
            _domainService = domainService;
            _itemFactory = itemFactory;
            _itemSaver = itemSaver;
        }

        [HttpGet("{domainId}/{code}")]
        [ProducesResponseType(typeof(Item), 200)]
        [Authorize]
        public async Task<IActionResult> GetByCode([FromRoute] Guid? domainId, [FromRoute] string code)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || Guid.Empty.Equals(domainId.Value))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (string.IsNullOrEmpty(code))
                {
                    result = BadRequest("Missing item code parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        IItem item = await _itemFactory.GetByCode(_settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (item == null)
                            item = _itemFactory.Create(domainId.Value, code);
                        Mapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(mapper.Map<Item>(item));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{domainId}/{code}/Data")]
        [Authorize]
        public async Task<IActionResult> GetDataByCode([FromRoute] Guid? domainId, [FromRoute] string code)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || Guid.Empty.Equals(domainId.Value))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (string.IsNullOrEmpty(code))
                {
                    result = BadRequest("Missing item code parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        IItem item = await _itemFactory.GetByCode(_settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (item == null)
                            result = Ok(null);
                        else
                            result = Ok(item.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{domainId}/{code}/History")]
        [ProducesResponseType(typeof(ItemHistory[]), 200)]
        [Authorize]
        public async Task<IActionResult> GetHistoryByCode([FromRoute] Guid? domainId, [FromRoute] string code)
        {
            IActionResult result = null;
            try
            {
                if (!domainId.HasValue || Guid.Empty.Equals(domainId.Value))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (string.IsNullOrEmpty(code))
                {
                    result = BadRequest("Missing item code parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        IItem item = await _itemFactory.GetByCode(_settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (item == null)
                        {
                            result = Ok(new List<ItemHistory>());
                        }
                        else
                        {
                            Mapper mapper = MapperConfigurationFactory.CreateMapper();
                            result = Ok(
                                (await item.GetHistory(_settingsFactory.CreateCore(_settings.Value)))
                                .Select(mapper.Map<ItemHistory>));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("/api/[controller]Code/{domainId}")]
        [ProducesResponseType(typeof(Item), 200)]
        [Authorize]
        public async Task<IActionResult> GetCodes([FromRoute] Guid? domainId)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || Guid.Empty.Equals(domainId.Value))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        result = Ok(
                            await _itemFactory.GetCodes(_settingsFactory.CreateCore(_settings.Value), domainId.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{domainId}/{code}/Data")]
        [ProducesResponseType(typeof(Item), 200)]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] string code, [FromBody] dynamic itemData)
        {
            IActionResult result = null;
            try
            {
                if (!domainId.HasValue || Guid.Empty.Equals(domainId.Value))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (string.IsNullOrEmpty(code))
                {
                    result = BadRequest("Missing item code parameter value");
                }
                else
                {
                    ConfigCoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IItem innerItem = null;
                    Func<ConfigCoreSettings, IItemSaver, IItem, Task> save = (sttngs, svr, lkup) => svr.Update(sttngs, lkup);
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                        innerItem = await _itemFactory.GetByCode(settings, domainId.Value, code);
                    if (result == null && innerItem == null)
                    {
                        innerItem = _itemFactory.Create(domainId.Value, code);
                        save = (sttngs, svr, lkup) => svr.Create(sttngs, lkup);
                    }
                    if (result == null && innerItem != null)
                    {
                        innerItem.Data = itemData;
                        await save(settings, _itemSaver, innerItem);
                        Mapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(
                            mapper.Map<Item>(innerItem));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpDelete("{domainId}/{code}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] Guid? domainId, [FromRoute] string code)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || Guid.Empty.Equals(domainId.Value))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (string.IsNullOrEmpty(code))
                {
                    result = BadRequest("Missing item code parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        await _itemSaver.DeleteByCode(_settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        result = Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
