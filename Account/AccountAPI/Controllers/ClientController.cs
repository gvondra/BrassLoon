using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Account.Framework.Enumerations;
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

        public ClientController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<ClientController> logger,
            MapperFactory mapperFactory,
            IClientFactory clientFactory,
            IClientSaver clientSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _clientSaver = clientSaver;
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
                    IEnumerable<IClient> clients = await _clientFactory.GetByAccountId(settings, id.Value);
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        clients.Select(mapper.Map<Client>)
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
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing client id value");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IClient client = await _clientFactory.Get(settings, id.Value);
                    if (client == null)
                        result = NotFound();
                    else if (!UserCanAccessAccount(client.AccountId))
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
            IActionResult result;
            try
            {
                if (client == null)
                {
                    result = BadRequest("Missing client data");
                }
                else if (string.IsNullOrEmpty(client.Name))
                {
                    result = BadRequest("Missing client name value");
                }
                else if (!client.AccountId.HasValue || client.AccountId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing account id value");
                }
                else if (!UserCanAccessAccount(client.AccountId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else if (string.IsNullOrEmpty(client.Secret))
                {
                    result = BadRequest("Missing secret value");
                }
                else if (client.Secret.Trim().Length < 16)
                {
                    result = BadRequest("Client secret must be at least 16 characters in lenth");
                }
                else
                {
                    IClient innerClient = await _clientFactory.Create(client.AccountId.Value, client.Secret, _settings.Value.SecretType);
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map<Client, IClient>(client, innerClient);
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
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing client id value");
                }
                else if (client == null)
                {
                    result = BadRequest("Missing client data");
                }
                else if (string.IsNullOrEmpty(client.Name))
                {
                    result = BadRequest("Missing client name value");
                }
                else if (!string.IsNullOrEmpty(client.Secret) && client.Secret.Trim().Length < 16)
                {
                    result = BadRequest("Client secret must be at least 16 characters in lenth");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IClient innerClient = await _clientFactory.Get(settings, id.Value);
                    if (innerClient == null)
                    {
                        result = NotFound();
                    }
                    else if (!UserCanAccessAccount(innerClient.AccountId))
                    {
                        result = StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        _ = mapper.Map<Client, IClient>(client, innerClient);
                        if (_settings.Value.SecretType == SecretType.Argon2 && !string.IsNullOrEmpty(client.Secret))
                            innerClient.SetSecret(client.Secret, _settings.Value.SecretType);
                        await _clientSaver.Update(
                            settings,
                            innerClient,
                            _settings.Value.SecretType != SecretType.Argon2 ? client.Secret : string.Empty);
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
