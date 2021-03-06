﻿using Autofac;
using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Interface.Account.Models;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IOptions<Settings> _settings;
        private readonly IContainer _container;

        public DomainController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
        }

        [HttpGet("/api/Account/{id}/Domain")]
        [ProducesResponseType(typeof(Domain[]), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> GetByAccountId([FromRoute] Guid? id, [FromQuery] bool deleted = false)
        {
            IActionResult result = null;
            try
            {
                if (result == null && !id.HasValue || id.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing account id value");
                if (result == null && !UserCanAccessAccount(id.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                    IDomainFactory domainFactory = scope.Resolve<IDomainFactory>();
                    IEnumerable<IDomain> domains;
                    if (deleted)
                        domains = await domainFactory.GetDeletedByAccountId(settings, id.Value);
                    else
                        domains = await domainFactory.GetByAccountId(settings, id.Value);
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    result = Ok(
                        domains.Select<IDomain, Domain>(d => mapper.Map<Domain>(d))
                        );
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && !id.HasValue || id.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing domain id value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                    IDomainFactory domainFactory = scope.Resolve<IDomainFactory>();
                    IDomain innerDomain = await domainFactory.Get(settings, id.Value);
                    if (innerDomain != null && !UserCanAccessAccount(innerDomain.AccountId))
                        innerDomain = null;
                    if (innerDomain == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(mapper.Map<Domain>(innerDomain));
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

        [HttpGet("/api/AccountDomain/{id}")]
        [ProducesResponseType(typeof(AccountDomain), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> GetAccountDomain([FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && !id.HasValue || id.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing domain id value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                    IDomainFactory domainFactory = scope.Resolve<IDomainFactory>();
                    IDomain innerDomain = await domainFactory.Get(settings, id.Value);
                    if (innerDomain != null && !UserCanAccessAccount(innerDomain.AccountId))
                        innerDomain = null;
                    if (innerDomain == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        AccountDomain accountDomain = mapper.Map<AccountDomain>(innerDomain);
                        IAccountFactory accountFactory = scope.Resolve<IAccountFactory>();
                        accountDomain.Account = mapper.Map<Account>(await accountFactory.Get(settings, innerDomain.AccountId));
                        result = Ok(accountDomain);
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

        [HttpPost()]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Create([FromBody] Domain domain)
        {
            IActionResult result = null;
            try
            {
                if (result == null && domain == null)
                    result = BadRequest("Missing domain data");
                if (result == null && string.IsNullOrEmpty(domain.Name))
                    result = BadRequest("Missing domain name value");
                if (result == null && (!domain.AccountId.HasValue || domain.AccountId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing account id value");
                if (result == null && !UserCanAccessAccount(domain.AccountId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                    IDomainFactory domainFactory = scope.Resolve<IDomainFactory>();
                    IDomain innerDomain = await domainFactory.Create(domain.AccountId.Value);
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    mapper.Map<Domain, IDomain>(domain, innerDomain);
                    IDomainSaver saver = scope.Resolve<IDomainSaver>();
                    await saver.Create(settings, innerDomain);
                    result = Ok(mapper.Map<Domain>(innerDomain));
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

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] Domain domain)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id value");
                if (result == null && domain == null)
                    result = BadRequest("Missing domain data");
                if (result == null && string.IsNullOrEmpty(domain.Name))
                    result = BadRequest("Missing domain name value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                    IDomainFactory domainFactory = scope.Resolve<IDomainFactory>();
                    IDomain innerDomain = await domainFactory.Get(settings, id.Value);
                    if (result == null && innerDomain == null)
                        result = NotFound();
                    if (result == null && !UserCanAccessAccount(innerDomain.AccountId))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                    {
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        mapper.Map<Domain, IDomain>(domain, innerDomain);
                        IDomainSaver saver = scope.Resolve<IDomainSaver>();
                        await saver.Update(settings, innerDomain);
                        result = Ok(mapper.Map<Domain>(innerDomain));
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

        [HttpPatch("{id}/Deleted")]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> UpdateDeleted([FromRoute] Guid? id, [FromBody] Dictionary<string, string> patch)
        {
            bool deleted = default;
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id value");
                if (result == null && patch == null)
                    result = BadRequest("Missing patch data");
                if (result == null && !patch.ContainsKey("Deleted"))
                    result = BadRequest("Missing deleted patch value");
                if (result == null && !bool.TryParse(patch["Deleted"], out deleted))
                    result = BadRequest("Invalid deleted patch value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                    IDomainFactory domainFactory = scope.Resolve<IDomainFactory>();
                    IDomain innerDomain;
                    if (!deleted)
                        innerDomain = await domainFactory.GetDeleted(settings, id.Value);
                    else
                        innerDomain = await domainFactory.Get(settings, id.Value);
                    if (result == null && innerDomain == null)
                        result = NotFound();
                    if (result == null && !UserCanAccessAccount(innerDomain.AccountId))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                    {
                        innerDomain.Deleted = deleted;
                        IDomainSaver saver = scope.Resolve<IDomainSaver>();
                        await saver.Update(settings, innerDomain);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(mapper.Map<Domain>(innerDomain));
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
