﻿using AutoMapper;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Authorization.Models;
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

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : AuthorizationContollerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IClientFactory _clientFactory;
        private readonly IClientSaver _clientSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly ISecretGenerator _secretGenerator;

        public ClientController(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<ClientController> logger,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IClientFactory clientFactory,
            IClientSaver clientSaver,
            IEmailAddressFactory emailAddressFactory,
            ISecretGenerator secretGenerator)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _clientSaver = clientSaver;
            _emailAddressFactory = emailAddressFactory;
            _secretGenerator = secretGenerator;
        }

        [HttpGet("{domainId}/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing or invalid domain id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = Unauthorized();
                }
                else
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IClient innerClient = await _clientFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerClient == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(await MapClient(coreSettings, mapper, innerClient));
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

        [HttpGet("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> GetByDomain([FromRoute] Guid? domainId)
        {
            IActionResult result = null;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing or invalid domain id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = Unauthorized();
                }
                else
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IMapper mapper = CreateMapper();
                    IEnumerable<IClient> innerClients = await _clientFactory.GetByDomainId(coreSettings, domainId.Value);
                    result = Ok(await Task.WhenAll(
                        innerClients.Select(c => MapClient(coreSettings, mapper, c))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("/api/ClientCredentialSecret")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetClientCredentialSecret()
        {
            IActionResult result;
            try
            {
                result = Ok(_secretGenerator.GenerateSecret());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        private IActionResult Validate(Client client)
        {
            IActionResult result = null;
            if (string.IsNullOrEmpty(client?.Name))
                result = BadRequest("Client name is required.");
            else if (client.Secret != null && client.Secret.Length < 32)
                result = BadRequest("Client secret must be at least 32 characters in length");
            return result;
        }

        [HttpPost("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, Client client)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing or invalid domain id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = Unauthorized();
                else
                    result = Validate(client);
                if (result == null && string.IsNullOrEmpty(client?.Secret))
                    result = BadRequest("Client secrect is required when creating a new client");
                if (result == null && domainId.HasValue && client != null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IClient innerClient = _clientFactory.Create(domainId.Value, client.Secret);
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(client, innerClient);
                    IEmailAddress userEmailAddress = await ConfigureUserEmailAddress(coreSettings, innerClient, client.UserEmailAddress);
                    if (client.Roles != null)
                        await ApplyRoleChanges(coreSettings, innerClient, client.Roles);
                    await _clientSaver.Create(coreSettings, innerClient, userEmailAddress);
                    result = Ok(
                        await MapClient(coreSettings, mapper, innerClient));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{domainId}/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, Client client)
        {
            IActionResult result;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                IClient innerClient = null;
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing or invalid domain id parameter value");
                else
                    result = Validate(client);
                if (result == null && domainId.HasValue && !await VerifyDomainAccount(domainId.Value))
                    result = Unauthorized();
                if (result == null && domainId.HasValue)
                {
                    innerClient = await _clientFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerClient == null)
                        result = NotFound();
                }
                if (result == null && innerClient != null)
                {
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(client, innerClient);
                    IEmailAddress userEmailAddress = await ConfigureUserEmailAddress(coreSettings, innerClient, client.UserEmailAddress);
                    if (!string.IsNullOrEmpty(client.Secret))
                        innerClient.SetSecret(client.Secret);
                    if (client.Roles != null)
                        await ApplyRoleChanges(coreSettings, innerClient, client.Roles);
                    await _clientSaver.Update(coreSettings, innerClient, userEmailAddress);
                    result = Ok(
                        await MapClient(coreSettings, mapper, innerClient));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        // returns an email address when creating a new email address. the returned email address needs to be saved
        [NonAction]
        private async Task<IEmailAddress> ConfigureUserEmailAddress(CoreSettings settings, IClient client, string emailAddress)
        {
            IEmailAddress result = null;
            if (string.IsNullOrEmpty(emailAddress))
            {
                client.SetUserEmailAddress(null);
            }
            else
            {
                result = await _emailAddressFactory.GetByAddress(settings, emailAddress);
                client.SetUserEmailAddress(result);
                if (!result.IsNew)
                    result = null;
            }
            return result;
        }

        [NonAction]
        private static async Task<Client> MapClient(CoreSettings coreSettings, IMapper mapper, IClient innerClient)
        {
            Client client = mapper.Map<Client>(innerClient);
            client.UserEmailAddress = (await innerClient.GetUserEmailAddress(coreSettings))?.Address ?? string.Empty;
            client.Roles = (await innerClient.GetRoles(coreSettings))
                .Select(mapper.Map<AppliedRole>)
                .ToList();
            return client;
        }

        [NonAction]
        private static async Task ApplyRoleChanges(CoreSettings coreSettings, IClient innerClient, List<AppliedRole> roles)
        {
            if (roles != null)
            {
                List<IRole> currentRoles = (await innerClient.GetRoles(coreSettings)).ToList();
                foreach (IRole currentRole in currentRoles)
                {
                    if (!roles.Exists(r => string.Equals(currentRole.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerClient.RemoveRole(coreSettings, currentRole.PolicyName);
                }
                foreach (AppliedRole role in roles)
                {
                    if (!currentRoles.Exists(r => string.Equals(role.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerClient.AddRole(coreSettings, role.PolicyName);
                }
            }
        }
    }
}
