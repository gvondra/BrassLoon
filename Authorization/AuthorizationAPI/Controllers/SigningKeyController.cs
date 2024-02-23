using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SigningKeyController : AuthorizationContollerBase
    {
        private readonly ILogger<SigningKeyController> _logger;
        private readonly ISigningKeyFactory _signingKeyFactory;
        private readonly ISigningKeySaver _signingKeySaver;
        public SigningKeyController(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<SigningKeyController> logger,
            MapperFactory mapperFactory,
            IDomainService domainService,
            ISigningKeyFactory signingKeyFactory,
            ISigningKeySaver signingKeySaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _logger = logger;
            _signingKeyFactory = signingKeyFactory;
            _signingKeySaver = signingKeySaver;
        }

        [HttpGet("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> GetByDomain([FromRoute] Guid? domainId)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && !await VerifyDomainAccount(domainId.Value))
                    result = Unauthorized();
                if (result == null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IMapper mapper = CreateMapper();
                    IEnumerable<ISigningKey> innerSigningKeys = await _signingKeyFactory.GetByDomainId(coreSettings, domainId.Value);
                    result = Ok(innerSigningKeys.Select(mapper.Map<SigningKey>));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPost("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, SigningKey signingKey)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && !await VerifyDomainAccount(domainId.Value))
                    result = Unauthorized();
                if (result == null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    ISigningKey innerSigningKey = _signingKeyFactory.Create(domainId.Value);
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(signingKey, innerSigningKey);
                    await _signingKeySaver.Create(coreSettings, innerSigningKey);
                    result = Ok(
                        mapper.Map<SigningKey>(innerSigningKey));
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
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, SigningKey signingKey)
        {
            IActionResult result = null;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                ISigningKey innerSigningKey = null;
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && !await VerifyDomainAccount(domainId.Value))
                    result = Unauthorized();
                if (result == null)
                {
                    innerSigningKey = await _signingKeyFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerSigningKey == null)
                        result = NotFound();
                }
                if (result == null && innerSigningKey != null)
                {
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(signingKey, innerSigningKey);
                    await _signingKeySaver.Update(coreSettings, innerSigningKey);
                    result = Ok(
                        mapper.Map<SigningKey>(innerSigningKey));
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
