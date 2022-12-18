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
using System.Threading.Tasks;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : AuthorizationContollerBase
    {
        private readonly IUserFactory _userFactory;
        private readonly IUserSaver _userSaver;

        public UserController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IUserFactory userFactory,
            IUserSaver userSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        { 
            _userFactory = userFactory;
            _userSaver = userSaver;
        }

        [HttpGet("{domainId}")]
        [Authorize()]
        [ProducesResponseType(typeof(List<User>), 200)]
        public async Task<IActionResult> Search([FromRoute] Guid? domainId, [FromQuery] string emailAddress)
        {
            IActionResult result = null;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && string.IsNullOrEmpty(emailAddress))
                    result = BadRequest("Missing emailAddress parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                {
                    IMapper mapper = CreateMapper();
                    IUser innerUser = await _userFactory.GetByEmailAddress(coreSettings, domainId.Value, emailAddress);
                    List<IUser> innerUsers = new List<IUser>();
                    if (innerUser != null)
                        innerUsers.Add(innerUser);
                    IEnumerable<Task<User>> users = innerUsers.Select<IUser, Task<User>>(u => MapUser(coreSettings, mapper, u));
                    result = Ok(
                        await Task.WhenAll<User>(users)
                        );
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpGet("{domainId}/{id}")]
        [Authorize()]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IUser innerUser = await _userFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerUser == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            await MapUser(coreSettings, mapper, innerUser)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpGet("{domainId}/{id}/Name")]
        [Authorize()]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetName([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                {
                    CoreSettings coreSettings = CreateCoreSettings();
                    IUser innerUser = await _userFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerUser == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        result = Ok(innerUser.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpPut("{domainId}/{id}")]
        [Authorize()]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromBody] User user)
        {
            IActionResult result = null;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                IUser innerUser = null;
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid domain id parameter value");
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing or invalid id parameter value");
                if (result == null && string.IsNullOrEmpty(user?.Name))
                    result = BadRequest("Missing user name value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = Unauthorized();
                if (result == null)
                {
                    innerUser = await _userFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerUser == null)
                        result = NotFound();
                }
                if (result == null && innerUser != null)
                {
                    IMapper mapper = CreateMapper();
                    mapper.Map(user, innerUser);
                    if (user.Roles != null)
                        await ApplyRoleChanges(coreSettings, innerUser, user.Roles);
                    await _userSaver.Update(coreSettings, innerUser);
                    result = Ok(
                        await MapUser(coreSettings, mapper, innerUser)
                        );
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [NonAction]
        private async Task<User> MapUser(CoreSettings coreSettings, IMapper mapper, IUser innerUser)
        {
            User user = mapper.Map<User>(innerUser);
            IEmailAddress emailAddress = await innerUser.GetEmailAddress(coreSettings);
            user.EmailAddress = emailAddress?.Address ?? string.Empty;
            user.Roles = (await innerUser.GetRoles(coreSettings))
                .Select<IRole, AppliedRole>(r => mapper.Map<AppliedRole>(r))
                .ToList();
            return user;
        }

        [NonAction]
        private async Task ApplyRoleChanges(CoreSettings coreSettings, IUser innerUser, List<AppliedRole> roles)
        {
            if (roles != null)
            {
                List<IRole> currentRoles = (await innerUser.GetRoles(coreSettings)).ToList();
                foreach (IRole currentRole in currentRoles)
                {
                    if (!roles.Any(r => string.Equals(currentRole.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerUser.RemoveRole(coreSettings, currentRole.PolicyName);
                }
                foreach (AppliedRole role in roles)
                {
                    if (!currentRoles.Any(r => string.Equals(role.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerUser.AddRole(coreSettings, role.PolicyName);
                }
            }
        }
    }
}
