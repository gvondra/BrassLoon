using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Interface.Account.Models;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DomainController : AccountControllerBase
    {
        private readonly ILogger<DomainController> _logger;
        private readonly IAccountFactory _accountFactory;
        private readonly IDomainFactory _domainFactory;
        private readonly IDomainSaver _domainSaver;

        public DomainController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<DomainController> logger,
            MapperFactory mapperFactory,
            IAccountFactory accountFactory,
            IDomainFactory domainFactory,
            IDomainSaver domainSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory)
        {
            _logger = logger;
            _accountFactory = accountFactory;
            _domainFactory = domainFactory;
            _domainSaver = domainSaver;
        }

        [HttpGet("/api/Account/{id}/Domain")]
        [ProducesResponseType(typeof(Domain[]), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> GetByAccountId([FromRoute] Guid? id, [FromQuery] bool deleted = false)
        {
            IActionResult result = null;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing account id value");
                }
                else if (!UserCanAccessAccount(id.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IEnumerable<IDomain> domains;
                    if (deleted)
                        domains = await _domainFactory.GetDeletedByAccountId(settings, id.Value);
                    else
                        domains = await _domainFactory.GetByAccountId(settings, id.Value);
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        domains.Select(mapper.Map<Domain>)
                        );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id value");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IDomain innerDomain = await _domainFactory.Get(settings, id.Value);
                    if (innerDomain != null && !UserCanAccessAccount(innerDomain.AccountId))
                        innerDomain = null;
                    if (innerDomain == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(mapper.Map<Domain>(innerDomain));
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

        [HttpGet("/api/AccountDomain/{id}")]
        [ProducesResponseType(typeof(AccountDomain), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> GetAccountDomain([FromRoute] Guid? id)
        {
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id value");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IDomain innerDomain = await _domainFactory.Get(settings, id.Value);
                    if (innerDomain != null && !UserCanAccessAccount(innerDomain.AccountId))
                        innerDomain = null;
                    if (innerDomain == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        AccountDomain accountDomain = mapper.Map<AccountDomain>(innerDomain);
                        accountDomain.Account = mapper.Map<Account>(await _accountFactory.Get(settings, innerDomain.AccountId));
                        result = Ok(accountDomain);
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

        [HttpPost()]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Create([FromBody] Domain domain)
        {
            IActionResult result;
            try
            {
                if (domain == null)
                {
                    result = BadRequest("Missing domain data");
                }
                else if (string.IsNullOrEmpty(domain.Name))
                {
                    result = BadRequest("Missing domain name value");
                }
                else if (!domain.AccountId.HasValue || domain.AccountId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing account id value");
                }
                else if (!UserCanAccessAccount(domain.AccountId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IDomain innerDomain = await _domainFactory.Create(domain.AccountId.Value);
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(domain, innerDomain);
                    await _domainSaver.Create(settings, innerDomain);
                    result = Ok(mapper.Map<Domain>(innerDomain));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] Domain domain)
        {
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id value");
                }
                else if (domain == null)
                {
                    result = BadRequest("Missing domain data");
                }
                else if (string.IsNullOrEmpty(domain.Name))
                {
                    result = BadRequest("Missing domain name value");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IDomain innerDomain = await _domainFactory.Get(settings, id.Value);
                    if (innerDomain == null)
                    {
                        result = NotFound();
                    }
                    else if (!UserCanAccessAccount(innerDomain.AccountId))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        _ = mapper.Map(domain, innerDomain);
                        await _domainSaver.Update(settings, innerDomain);
                        result = Ok(mapper.Map<Domain>(innerDomain));
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

        [HttpPatch("{id}/Deleted")]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> UpdateDeleted([FromRoute] Guid? id, [FromBody] Dictionary<string, string> patch)
        {
            bool deleted;
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id value");
                }
                else if (patch == null)
                {
                    result = BadRequest("Missing patch data");
                }
                else if (!patch.ContainsKey("Deleted"))
                {
                    result = BadRequest("Missing deleted patch value");
                }
                else if (!bool.TryParse(patch["Deleted"], out deleted))
                {
                    result = BadRequest("Invalid deleted patch value");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IDomain innerDomain;
                    if (!deleted)
                        innerDomain = await _domainFactory.GetDeleted(settings, id.Value);
                    else
                        innerDomain = await _domainFactory.Get(settings, id.Value);
                    if (innerDomain == null)
                    {
                        result = NotFound();
                    }
                    else if (!UserCanAccessAccount(innerDomain.AccountId))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        innerDomain.Deleted = deleted;
                        await _domainSaver.Update(settings, innerDomain);
                        IMapper mapper = CreateMapper();
                        result = Ok(mapper.Map<Domain>(innerDomain));
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
