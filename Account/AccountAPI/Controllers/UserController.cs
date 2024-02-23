using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Account.Framework.Enumerations;
using BrassLoon.Interface.Account.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Log = BrassLoon.Interface.Log;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : AccountControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserFactory _userFactory;
        private readonly IUserSaver _userSaver;

        public UserController(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Log.IExceptionService exceptionService,
            ILogger<UserController> logger,
            MapperFactory mapperFactory,
            IUserFactory userFactory,
            IUserSaver userSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory)
        {
            _logger = logger;
            _userFactory = userFactory;
            _userSaver = userSaver;
        }

        [HttpGet]
        [Authorize("ADMIN:SYS")]
        public async Task<IActionResult> Search([FromQuery] string emailAddress)
        {
            IActionResult result = null;
            try
            {
                IEnumerable<User> users = null;
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IMapper mapper = CreateMapper();
                    users = (await _userFactory.GetByEmailAddress(settings, emailAddress))
                        .Select(mapper.Map<User>);
                }
                if (users == null)
                    users = new List<User>();
                result = Ok(users.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{id}")]
        [Authorize("ADMIN:SYS")]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                IUser user = null;
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else
                {
                    user = await _userFactory.Get(settings, id.Value);
                    if (user == null)
                        result = NotFound();
                }
                if (result == null && user != null)
                {
                    IMapper mapper = CreateMapper();
                    result = Ok(mapper.Map<User>(user));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{id}/Role")]
        [Authorize("ADMIN:SYS")]
        public async Task<IActionResult> GetRoles([FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                IUser user = null;
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else
                {
                    user = await _userFactory.Get(settings, id.Value);
                    if (user == null)
                        result = NotFound();
                }
                if (result == null && user != null)
                {
                    List<string> roles = new List<string>();
                    if ((user.Roles & UserRole.SystemAdministrator) == UserRole.SystemAdministrator)
                        roles.Add("sysadmin");
                    if ((user.Roles & UserRole.AccountAdministrator) == UserRole.AccountAdministrator)
                        roles.Add("actadmin");
                    result = Ok(roles);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{id}/Role")]
        [Authorize("ADMIN:SYS")]
        public async Task<IActionResult> SaveRoles([FromRoute] Guid? id, [FromBody] List<string> roles)
        {
            IActionResult result = null;
            try
            {
                roles = roles ?? new List<string>();
                CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                IUser user = null;
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else
                {
                    user = await _userFactory.Get(settings, id.Value);
                    if (user == null)
                        result = NotFound();
                }
                if (result == null && user != null)
                {
                    IUser currentUser = await GetUser(_userFactory, settings);
                    if (currentUser.UserId.Equals(id.Value))
                        result = Unauthorized();
                }
                if (result == null && user != null)
                {
                    if (roles.Exists(r => string.Equals(r, "sysadmin", StringComparison.OrdinalIgnoreCase)))
                        user.Roles = user.Roles | UserRole.SystemAdministrator;
                    else
                        user.Roles = user.Roles & (~UserRole.SystemAdministrator);
                    if (roles.Exists(r => string.Equals(r, "actadmin", StringComparison.OrdinalIgnoreCase)))
                        user.Roles = user.Roles | UserRole.AccountAdministrator;
                    else
                        user.Roles = user.Roles & (~UserRole.AccountAdministrator);
                    await _userSaver.Update(settings, user);
                    result = Ok();
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
