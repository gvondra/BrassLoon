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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : AuthorizationContollerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleFactory _roleFactory;
        private readonly IRoleSaver _roleSaver;

        public RoleController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<RoleController> logger,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IRoleFactory roleFactory,
            IRoleSaver roleSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _logger = logger;
            _roleFactory = roleFactory;
            _roleSaver = roleSaver;
        }

        [HttpGet("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<Role>), 200)]
        public async Task<IActionResult> GetByDomainId([FromRoute] Guid? domainId)
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
                    result = Ok(
                        (await _roleFactory.GetByDomainId(coreSettings, domainId.Value))
                        .Select(mapper.Map<Role>)
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

        [NonAction]
        private IActionResult ValidateCreate(Role role)
        {
            IActionResult result = Validate(role);
            if (result == null && string.IsNullOrEmpty(role.PolicyName))
                result = BadRequest("Missing role policy name value");
            return result;
        }

        [NonAction]
        private IActionResult Validate(Role role)
        {
            IActionResult result = null;
            if (role == null)
                result = BadRequest("Missing role data body");
            else if (string.IsNullOrEmpty(role.Name))
                result = BadRequest("Missing role name value");
            return result;
        }

        [NonAction]
        private async Task<IActionResult> ValidatePolicyNameNotExists(CoreSettings coreSettings, Guid domainId, Role role)
        {
            IActionResult result = null;
            if ((await _roleFactory.GetByDomainId(coreSettings, domainId)).Any(r => string.Equals(role.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                result = BadRequest($"A role with policy name \"{role.PolicyName}\" already exists");
            return result;
        }

        [HttpPost("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(Role), 200)]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, [FromBody] Role role)
        {
            IActionResult result;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing or invalid domain id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = Unauthorized();
                else
                    result = ValidateCreate(role);
                if (result == null && domainId.HasValue)
                    result = await ValidatePolicyNameNotExists(coreSettings, domainId.Value, role);
                if (result == null && domainId.HasValue)
                {
                    IRole innerRole = _roleFactory.Create(domainId.Value, role.PolicyName);
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(role, innerRole);
                    await _roleSaver.Create(coreSettings, innerRole);
                    result = Ok(mapper.Map<Role>(innerRole));
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
        [ProducesResponseType(typeof(Role), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromBody] Role role)
        {
            IActionResult result;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                IRole innerRole = null;
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing or invalid domain id parameter value");
                else if (!id.HasValue || id.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing or invalid role id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = Unauthorized();
                else
                    result = Validate(role);
                if (result == null && domainId.HasValue && id.HasValue)
                {
                    innerRole = await _roleFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerRole == null)
                        result = NotFound();
                }
                if (result == null && innerRole != null)
                {
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(role, innerRole);
                    await _roleSaver.Update(coreSettings, innerRole);
                    result = Ok(mapper.Map<Role>(innerRole));
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
