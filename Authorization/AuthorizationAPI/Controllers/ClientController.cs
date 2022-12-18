using AutoMapper;
using BrassLoon.Authorization.Framework;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Authorization.Models;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings;
using System.Threading.Tasks;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : AuthorizationContollerBase
    {
        private readonly IClientFactory _clientFactory;
        private readonly IClientSaver _clientSaver;

        public ClientController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IClientFactory clientFactory,
            IClientSaver clientSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _clientFactory = clientFactory;
            _clientSaver = clientSaver;
        }

        [HttpGet("{domainId}/{id}")]
        [Authorize()]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IClient innerClient = await _clientFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerClient == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(await MapClient(coreSettings, mapper, innerClient));
                    }   
                }
            }
            catch(Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{domainId}")]
        [Authorize()]
        public async Task<IActionResult> GetByDomain([FromRoute] Guid? domainId)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IMapper mapper = CreateMapper();
                    IEnumerable<IClient> innerClients = await _clientFactory.GetByDomainId(coreSettings, domainId.Value);
                    result = Ok(await Task.WhenAll<Client>(
                        innerClients.Select<IClient, Task<Client>>(c => MapClient(coreSettings, mapper, c))
                        ));
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("/api/ClientCredentialSecret")]
        [Authorize()]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetClientCredentialSecret()
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
                    Encoding.ASCII.GetString(values)
                    );
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        private IActionResult Validate(Client client)
        {
            IActionResult result = null;
            if (result == null && string.IsNullOrEmpty(client?.Name))
                result = BadRequest("Client name is required.");
            if (result == null && client?.Secret != null && client.Secret.Length < 32)
                result = BadRequest("Client secret must be at least 32 characters in length");
            return result;
        }

        [HttpPost("{domainId}")]
        [Authorize()]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, Client client)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                    result = Validate(client);
                if (result == null && string.IsNullOrEmpty(client?.Secret))
                    result = BadRequest("Client secrect is required when creating a new client");
                if (result == null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IClient innerClient = _clientFactory.Create(domainId.Value, client.Secret);
                    IMapper mapper = CreateMapper();
                    mapper.Map<Client, IClient>(client, innerClient);
                    innerClient.SetSecret(client.Secret);
                    if (client.Roles != null)
                        await ApplyRoleChanges(coreSettings, innerClient, client.Roles);
                    await _clientSaver.Create(coreSettings, innerClient);
                    result = Ok(
                        MapClient(coreSettings, mapper, innerClient)
                        );
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{domainId}/{id}")]
        [Authorize()]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, Client client)
        {
            IActionResult result = null;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                IClient innerClient = null;
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null)
                    result = Validate(client);
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                {
                    innerClient = await _clientFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerClient == null)
                        result = NotFound();
                }
                if (result == null && innerClient != null)
                {
                    IMapper mapper = CreateMapper();
                    mapper.Map(client, innerClient);
                    if (!string.IsNullOrEmpty(client?.Secret))
                        innerClient.SetSecret(client.Secret);
                    if (client.Roles != null)
                        await ApplyRoleChanges(coreSettings, innerClient, client.Roles);
                    await _clientSaver.Update(coreSettings, innerClient);
                    result = Ok(
                        MapClient(coreSettings, mapper, innerClient)
                        );
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        private async Task<Client> MapClient(CoreSettings coreSettings, IMapper mapper, IClient innerClient)
        {
            Client client = mapper.Map<Client>(innerClient);
            client.Roles = (await innerClient.GetRoles(coreSettings))
                .Select<IRole, AppliedRole>(r => mapper.Map<AppliedRole>(r))
                .ToList();
            return client;
        }

        [NonAction]
        private async Task ApplyRoleChanges(CoreSettings coreSettings, IClient innerClient, List<AppliedRole> roles)
        {
            if (roles != null)
            {
                List<IRole> currentRoles = (await innerClient.GetRoles(coreSettings)).ToList();
                foreach (IRole currentRole in currentRoles)
                {
                    if (!roles.Any(r => string.Equals(currentRole.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerClient.RemoveRole(coreSettings, currentRole.PolicyName);
                }
                foreach (AppliedRole role in roles)
                {
                    if (!currentRoles.Any(r => string.Equals(role.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerClient.AddRole(coreSettings, role.PolicyName);
                }
            }
        }
    }
}
