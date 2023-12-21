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
    public class UserController : AuthorizationContollerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserFactory _userFactory;
        private readonly IUserSaver _userSaver;

        public UserController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<UserController> logger,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IUserFactory userFactory,
            IUserSaver userSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _logger = logger;
            _userFactory = userFactory;
            _userSaver = userSaver;
        }

        [HttpGet("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<User>), 200)]
        public async Task<IActionResult> Search(
            [FromRoute] Guid? domainId,
            [FromQuery] string emailAddress,
            [FromQuery] string referenceId)
        {
            IActionResult result = null;
            try
            {
                IUser innerUser = null;
                CoreSettings coreSettings = CreateCoreSettings();
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing or invalid domain id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = Unauthorized();
                if (result == null && !string.IsNullOrEmpty(emailAddress) && domainId.HasValue)
                {
                    innerUser = await _userFactory.GetByEmailAddress(coreSettings, domainId.Value, emailAddress);
                }
                if (result == null && innerUser == null && !string.IsNullOrEmpty(referenceId) && domainId.HasValue)
                {
                    innerUser = await _userFactory.GetByReferenceId(coreSettings, domainId.Value, referenceId);
                }
                if (result == null && innerUser == null && domainId.HasValue)
                {
                    innerUser = await _userFactory.GetByReferenceId(coreSettings, domainId.Value, GetCurrentUserReferenceId());
                }
                List<IUser> innerUsers = null;
                if (result == null && innerUser != null)
                {
                    innerUsers = new List<IUser>()
                    {
                        innerUser
                    };
                }
                if (result == null && innerUsers != null)
                {
                    IMapper mapper = CreateMapper();
                    IEnumerable<Task<User>> users = innerUsers.Select(u => MapUser(coreSettings, mapper, u));
                    result = Ok(
                        await Task.WhenAll(users)
                        );
                }
                if (result == null)
                {
                    result = Ok(new List<User>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpGet("{domainId}/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing or invalid domain id parameter value");
                }
                else if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing or invalid id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = Unauthorized();
                }
                else
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
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpGet("{domainId}/{id}/Name")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetName([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result;
            try
            {
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing or invalid domain id parameter value");
                }
                else if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing or invalid id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = Unauthorized();
                }
                else
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
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [HttpPut("{domainId}/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromBody] User user)
        {
            IActionResult result = null;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                IUser innerUser = null;
                if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing or invalid domain id parameter value");
                }
                else if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing or invalid id parameter value");
                }
                else if (string.IsNullOrEmpty(user?.Name))
                {
                    result = BadRequest("Missing user name value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = Unauthorized();
                }
                else
                {
                    innerUser = await _userFactory.Get(coreSettings, domainId.Value, id.Value);
                    if (innerUser == null)
                        result = NotFound();
                }
                if (result == null && innerUser != null)
                {
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(user, innerUser);
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
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }

        [NonAction]
        private static async Task<User> MapUser(CoreSettings coreSettings, IMapper mapper, IUser innerUser)
        {
            User user = mapper.Map<User>(innerUser);
            IEmailAddress emailAddress = await innerUser.GetEmailAddress(coreSettings);
            user.EmailAddress = emailAddress?.Address ?? string.Empty;
            user.Roles = (await innerUser.GetRoles(coreSettings))
                .Select(r => mapper.Map<AppliedRole>(r))
                .ToList();
            return user;
        }

        [NonAction]
        private static async Task ApplyRoleChanges(CoreSettings coreSettings, IUser innerUser, List<AppliedRole> roles)
        {
            if (roles != null)
            {
                List<IRole> currentRoles = (await innerUser.GetRoles(coreSettings)).ToList();
                foreach (IRole currentRole in currentRoles)
                {
                    if (!roles.Exists(r => string.Equals(currentRole.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerUser.RemoveRole(coreSettings, currentRole.PolicyName);
                }
                foreach (AppliedRole role in roles)
                {
                    if (!currentRoles.Exists(r => string.Equals(role.PolicyName, r.PolicyName, StringComparison.OrdinalIgnoreCase)))
                        await innerUser.AddRole(coreSettings, role.PolicyName);
                }
            }
        }
    }
}
