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
    public class ClientController : AccountControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly SettingsFactory _settingsFactory;
        private readonly Lazy<IExceptionService> _exceptionService;
        private readonly IClientFactory _clientFactory;
        private readonly IClientSaver _clientSaver;
        private readonly ISecretProcessor _secretProcessor;

        public ClientController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Lazy<IExceptionService> exceptionService,
            IClientFactory clientFactory,
            IClientSaver clientSaver,
            ISecretProcessor secretProcessor)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
            _exceptionService = exceptionService;
            _clientFactory = clientFactory;
            _clientSaver = clientSaver;
            _secretProcessor = secretProcessor;
        }

        [HttpGet("/api/ClientSecret")]
        public IActionResult CreateSecret()
        {
            return Ok(new { Secret = _secretProcessor.Create() });
        }

        [HttpGet("/api/Account/{id}/Client")]
        [ProducesResponseType(typeof(Client[]), 200)]        
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> GetByAccountId([FromRoute] Guid? id)
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
                    CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
                    IEnumerable<IClient> clients = await _clientFactory.GetByAccountId(settings, id.Value);
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    result = Ok(
                        clients.Select<IClient, Client>(d => mapper.Map<Client>(d))
                        );
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }            
            return result;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Client), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && !id.HasValue || id.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing client id value");
                if (result == null)
                {
                    CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
                    IClient client = await _clientFactory.Get(settings, id.Value);
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
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(Client), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Create([FromBody] ClientCredentialRequest client)
        {
            IActionResult result = null;
            try
            {
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
                    IClient innerClient = await _clientFactory.Create(client.AccountId.Value, client.Secret);
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    mapper.Map<Client, IClient>(client, innerClient);
                    CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
                    await _clientSaver.Create(settings, innerClient);
                    result = Ok(mapper.Map<Client>(innerClient));
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Client), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] ClientCredentialRequest client)
        {
            IActionResult result = null;
            try
            {
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
                    CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
                    IClient innerClient = await _clientFactory.Get(settings, id.Value);
                    if (innerClient == null)
                        result = NotFound();
                    if (result == null && !UserCanAccessAccount(innerClient.AccountId))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                    {
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        mapper.Map<Client, IClient>(client, innerClient);
                        await _clientSaver.Update(settings, innerClient, client.Secret);
                        result = Ok(mapper.Map<Client>(innerClient));
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
