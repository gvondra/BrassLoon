using AutoMapper;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonAPI;
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
using System.Threading.Tasks;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : AuthorizationContollerBase
    {
        private readonly IRoleFactory _roleFactory;
        private readonly IRoleSaver _roleSaver;

        public RoleController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IRoleFactory roleFactory,
            IRoleSaver roleSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService) 
        {
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
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        (await _roleFactory.GetByDomainId(coreSettings, domainId.Value))
                        .Select<IRole, Role>(r => mapper.Map<Role>(r))
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
            if (result == null && role == null)
                result = BadRequest("Missing role data body");
            if (result == null && string.IsNullOrEmpty(role.Name))
                result = BadRequest("Missing role name value");            
            return result;
        }

        [NonAction]
        private async Task<IActionResult> ValidatePolicyNameNotExists(CoreSettings coreSettings, Role role)
        {
            IActionResult result = null;
            if ((await _roleFactory.GetByDomainId(coreSettings, role.DomainId.Value)).Any(r => string.Equals(role.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                result = BadRequest($"A role with policy name \"{role.PolicyName}\" already exists");
            return result;
        }

        [HttpPost("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(Role), 200)]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, [FromBody] Role role)
        {
            IActionResult result = null;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                    result = ValidateCreate(role);
                if (result == null)
                    result = await ValidatePolicyNameNotExists(coreSettings, role);
                if (result == null)
                {
                    IRole innerRole = _roleFactory.Create(domainId.Value, role.PolicyName);
                    IMapper mapper = CreateMapper();
                    mapper.Map(role, innerRole);
                    await _roleSaver.Create(coreSettings, innerRole);
                    result = Ok(mapper.Map<Role>(innerRole));
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
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(Role), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromBody] Role role)
        {
            IActionResult result = null;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                IRole innerRole = null;
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid role id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                    result = Validate(role);
                if (result == null)
                {
                    innerRole = await _roleFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerRole == null)
                        result = NotFound();
                }
                if (result == null && innerRole != null)
                {
                    IMapper mapper = CreateMapper();
                    mapper.Map(role, innerRole);
                    await _roleSaver.Update(coreSettings, innerRole);
                    result = Ok(mapper.Map<Role>(innerRole)); 
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
