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
    public class LookupController : ConfigControllerBase
    {
        private readonly ILogger<LookupController> _logger;
        private readonly IDomainService _domainService;
        private readonly ILookupFactory _lookupFactory;
        private readonly ILookupSaver _lookupSaver;

        public LookupController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            ILogger<LookupController> logger,
            IDomainService domainService,
            ILookupFactory lookupFactory,
            ILookupSaver lookupSaver)
            : base(settings, settingsFactory)
        {
            _logger = logger;
            _domainService = domainService;
            _lookupFactory = lookupFactory;
            _lookupSaver = lookupSaver;
        }

        [HttpGet("{domainId}/{code}")]
        [ProducesResponseType(typeof(Lookup), 200)]
        [Authorize()]
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
                    result = BadRequest("Missing lookup code parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        ILookup lookup = await _lookupFactory.GetByCode(_settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (lookup == null)
                        {
                            result = NotFound();
                        }
                        else
                        {
                            Mapper mapper = MapperConfigurationFactory.CreateMapper();
                            result = Ok(mapper.Map<Lookup>(lookup));
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

        [HttpGet("{domainId}/{code}/Data")]
        [ProducesResponseType(typeof(Dictionary<string, string>), 200)]
        [Authorize()]
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
                    result = BadRequest("Missing lookup code parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        ILookup lookup = await _lookupFactory.GetByCode(_settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (lookup == null)
                        {
                            result = NotFound();
                        }
                        else
                        {
                            Mapper mapper = MapperConfigurationFactory.CreateMapper();
                            result = Ok(mapper.Map<Dictionary<string, string>>(lookup.Data));
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

        [HttpGet("{domainId}/{code}/History")]
        [ProducesResponseType(typeof(LookupHistory[]), 200)]
        [Authorize()]
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
                    result = BadRequest("Missing lookup code parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        ILookup lookup = await _lookupFactory.GetByCode(_settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
                        if (lookup == null)
                        {
                            result = NotFound();
                        }
                        else
                        {
                            Mapper mapper = MapperConfigurationFactory.CreateMapper();
                            result = Ok(
                                (await lookup.GetHistory(_settingsFactory.CreateCore(_settings.Value)))
                                .Select(mapper.Map<LookupHistory>));
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
        [ProducesResponseType(typeof(Lookup), 200)]
        [Authorize()]
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
                            await _lookupFactory.GetCodes(_settingsFactory.CreateCore(_settings.Value), domainId.Value)
                            );
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
        [ProducesResponseType(typeof(Lookup), 200)]
        [Authorize()]
        public async Task<IActionResult> Save([FromRoute] Guid? domainId, [FromRoute] string code, [FromBody] Dictionary<string, string> lookupData)
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
                    result = BadRequest("Missing lookup code parameter value");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    ILookup innerLookup = null;
                    Func<CoreSettings, ILookupSaver, ILookup, Task> save = (sttngs, svr, lkup) => svr.Update(sttngs, lkup);
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                        innerLookup = await _lookupFactory.GetByCode(settings, domainId.Value, code);
                    if (result == null && innerLookup == null)
                    {
                        innerLookup = _lookupFactory.Create(domainId.Value, code);
                        save = (sttngs, svr, lkup) => svr.Create(sttngs, lkup);
                    }
                    if (result == null && innerLookup != null)
                    {
                        innerLookup.Data = lookupData;
                        await save(settings, _lookupSaver, innerLookup);
                        Mapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(
                            mapper.Map<Lookup>(innerLookup)
                            );
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
        [Authorize()]
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
                    result = BadRequest("Missing lookup code parameter value");
                }
                else
                {
                    if (!await VerifyDomainAccount(domainId.Value, _settings.Value, _domainService))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        await _lookupSaver.DeleteByCode(_settingsFactory.CreateCore(_settings.Value), domainId.Value, code);
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