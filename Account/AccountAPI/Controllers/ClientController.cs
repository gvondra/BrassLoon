using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.CommonAPI;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : AccountControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IClientFactory _clientFactory;
        private readonly IClientSaver _clientSaver;
        private readonly ISecretProcessor _secretProcessor;

        public ClientController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<ClientController> logger,
            MapperFactory mapperFactory,
            IClientFactory clientFactory,
            IClientSaver clientSaver,
            ISecretProcessor secretProcessor)
            : base(settings, settingsFactory, exceptionService, mapperFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _clientSaver = clientSaver;
            _secretProcessor = secretProcessor;
        }

        [HttpGet("/api/ClientSecret")]
        [ProducesResponseType(typeof(string), 200)]
        [Authorize("READ:ACCOUNT")]
        public IActionResult CreateSecret()
        {
            IActionResult result;
            try
            {
                RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                byte firstCharacter = 33;
                byte characterRange = 94;
                byte[] values = new byte[32];
                randomNumberGenerator.GetBytes(values);
                for (int i = 0; i < values.Length; i += 1)
                {
                    values[i] = (byte)((values[i] % characterRange) + firstCharacter);
                }
                result = Ok(
                    Encoding.ASCII.GetString(values));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
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
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IEnumerable<IClient> clients = await _clientFactory.GetByAccountId(settings, id.Value);
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        clients.Select<IClient, Client>(d => mapper.Map<Client>(d))
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
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IClient client = await _clientFactory.Get(settings, id.Value);
                    if (client == null)
                        result = NotFound();
                    if (result == null && !UserCanAccessAccount(client.AccountId))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            mapper.Map<Client>(client)
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
                if (result == null && string.IsNullOrEmpty(client?.Secret))
                    result = BadRequest("Missing secret value");
                if (result == null && client.Secret.Trim().Length < 16)
                    result = BadRequest("Client secret must be at least 16 characters in lenth");
                if (result == null)
                {
                    IClient innerClient = await _clientFactory.Create(client.AccountId.Value, client.Secret);
                    IMapper mapper = CreateMapper();
                    mapper.Map<Client, IClient>(client, innerClient);
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    await _clientSaver.Create(settings, innerClient);
                    result = Ok(mapper.Map<Client>(innerClient));
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
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IClient innerClient = await _clientFactory.Get(settings, id.Value);
                    if (innerClient == null)
                        result = NotFound();
                    if (result == null && !UserCanAccessAccount(innerClient.AccountId))
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    if (result == null)
                    {
                        IMapper mapper = CreateMapper();
                        mapper.Map<Client, IClient>(client, innerClient);
                        if (!string.IsNullOrEmpty(client?.Secret))
                            innerClient.SetSecret(client.Secret);
                        await _clientSaver.Update(settings, innerClient, client.Secret);
                        result = Ok(mapper.Map<Client>(innerClient));
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
