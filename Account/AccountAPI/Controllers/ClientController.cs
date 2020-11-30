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
    public class ClientController : AccountControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly IContainer _container;

        public ClientController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
        }

        [HttpGet("/api/ClientSecret")]
        public IActionResult CreateSecret()
        {
            using (ILifetimeScope scope = _container.BeginLifetimeScope())
            {
                ISecretProcessor secretProcessor = scope.Resolve<ISecretProcessor>();
                return Ok(new { Secret = secretProcessor.Create() });
            }
        }

        [HttpGet("/api/Account/{id}/Client")]
        [ProducesResponseType(typeof(Client[]), 200)]        
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> GetByAccountId([FromRoute] Guid? id)
        {
            IActionResult result = null;
            if (result == null && !id.HasValue || id.Value.Equals(Guid.Empty))
                result = BadRequest("Missing account id value");
            if (result == null && !UserCanAccessAccount(id.Value))
                result = StatusCode(StatusCodes.Status401Unauthorized);
            if (result == null)
            {
                using ILifetimeScope scope = _container.BeginLifetimeScope();
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                IClientFactory clientFactory = scope.Resolve<IClientFactory>();
                IEnumerable<IClient> clients = await clientFactory.GetByAccountId(settings, id.Value);
                IMapper mapper = MapperConfigurationFactory.CreateMapper();
                result = Ok(
                    clients.Select<IClient, Client>(d => mapper.Map<Client>(d))
                    );
            }
            return result;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Client), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            IActionResult result = null;
            if (result == null && !id.HasValue || id.Value.Equals(Guid.Empty))
                result = BadRequest("Missing client id value");
            if (result == null)
            {
                using ILifetimeScope scope = _container.BeginLifetimeScope();
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                IClientFactory clientFactory = scope.Resolve<IClientFactory>();
                IClient client = await clientFactory.Get(settings, id.Value);
                if (client == null)
                    result = NotFound();
                if (result == null && !UserCanAccessAccount(client.AccountId))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    result = Ok(
                        mapper.Map<Client>(client)
                        );
                }                
            }
            return result;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(Client), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Create([FromBody] ClientCredentialRequest client)
        {
            IActionResult result = null;
            if (result == null && client == null)
                result = BadRequest("Missing client data");
            if (result == null && string.IsNullOrEmpty(client.Name))
                result = BadRequest("Missing client name value");
            if (result == null && (!client.AccountId.HasValue || client.AccountId.Value.Equals(Guid.Empty)))
                result = BadRequest("Missing account id value");
            if (result == null && !UserCanAccessAccount(client.AccountId.Value))
                result = StatusCode(StatusCodes.Status401Unauthorized);
            if (result == null && string.IsNullOrEmpty(client.Secret))
                result = BadRequest("Missing secret value");
            if (result == null && client.Secret.Trim().Length < 16)
                result = BadRequest("Client secret must be at least 16 characters in lenth");
            if (result == null)
            {
                using ILifetimeScope scope = _container.BeginLifetimeScope();
                IClientFactory clientFactory = scope.Resolve<IClientFactory>();
                IClient innerClient = await clientFactory.Create(client.AccountId.Value, client.Secret);
                IMapper mapper = MapperConfigurationFactory.CreateMapper();
                mapper.Map<Client, IClient>(client, innerClient);
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                IClientSaver saver = scope.Resolve<IClientSaver>();
                await saver.Create(settings, innerClient);
                result = Ok(mapper.Map<Client>(innerClient));
            }
            return result;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Client), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] ClientCredentialRequest client)
        {
            IActionResult result = null;
            if (result == null && !id.HasValue || id.Value.Equals(Guid.Empty))
                result = BadRequest("Missing client id value");
            if (result == null && client == null)
                result = BadRequest("Missing client data");
            if (result == null && string.IsNullOrEmpty(client.Name))
                result = BadRequest("Missing client name value");
            if (result == null && !string.IsNullOrEmpty(client.Secret) && client.Secret.Trim().Length < 16)
                result = BadRequest("Client secret must be at least 16 characters in lenth");
            if (result == null)
            {
                using ILifetimeScope scope = _container.BeginLifetimeScope();
                IClientFactory clientFactory = scope.Resolve<IClientFactory>();
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                IClient innerClient = await clientFactory.Get(settings, id.Value);
                if (innerClient == null)
                    result = NotFound();
                if (result == null && !UserCanAccessAccount(innerClient.AccountId))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    mapper.Map<Client, IClient>(client, innerClient);
                    IClientSaver saver = scope.Resolve<IClientSaver>();
                    await saver.Update(settings, innerClient, client.Secret);
                    result = Ok(mapper.Map<Client>(innerClient));
                }                
            }
            return result;
        }
    }
}
