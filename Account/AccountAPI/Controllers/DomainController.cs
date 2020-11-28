using Autofac;
using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Interface.Account.Models;
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            IActionResult result = null;
            if (result == null && !id.HasValue || id.Value.Equals(Guid.Empty))
                result = BadRequest("Missing domain id value");
            if (result == null)
            {
                using ILifetimeScope scope = _container.BeginLifetimeScope();
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                IDomainFactory domainFactory = scope.Resolve<IDomainFactory>();
                IDomain domain = await domainFactory.Get(settings, id.Value);
                if (domain == null)
                    result = NotFound();
                if (result == null && !UserCanAccessAccount(domain.AccountId))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    result = Ok(mapper.Map<Domain>(domain));
                }                
            }
            return result;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Create([FromBody] Domain domain)
        {
            IActionResult result = null;
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
            return result;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Domain), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] Domain domain)
        {
            IActionResult result = null;
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
            return result;
        }
    }
}
