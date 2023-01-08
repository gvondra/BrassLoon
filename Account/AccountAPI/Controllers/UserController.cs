using System;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using BrassLoon.Interface.Account.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Log = BrassLoon.Interface.Log;
using System.Linq;
using AutoMapper;
using BrassLoon.Account.Framework.Enumerations;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : AccountControllerBase
    {
        private readonly Lazy<Log.IExceptionService> _exceptionService;
        private readonly IUserFactory _userFactory;
        private readonly IUserSaver _userSaver;

        public UserController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Lazy<Log.IExceptionService> exceptionService,
            IUserFactory userFactory,
            IUserSaver userSaver)
            : base(settings, settingsFactory) 
        {
            _exceptionService = exceptionService;
            _userFactory = userFactory;
            _userSaver = userSaver;
        }

        [HttpGet()]
        [Authorize("ADMIN:SYS")]
        public async Task<IActionResult> Search([FromQuery] string emailAddress)
        {
            IActionResult result = null;
            try
            {
                IEnumerable<User> users = null;
                if (result == null && !string.IsNullOrEmpty(emailAddress))
                {
                    ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    users = (await _userFactory.GetByEmailAddress(settings, emailAddress))
                        .Select<IUser, User>(u => mapper.Map<User>(u));                    
                }
                if (result == null)
                {
                    if (users == null)
                        users = new List<User>();
                    result = Ok(users.ToList());
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
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
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                IUser user = null;
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null)
                {
                    user = await _userFactory.Get(settings, id.Value);
                    if (user == null)
                        result = NotFound();                        
                }
                if (result == null && user != null)
                {
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    result = Ok(mapper.Map<User>(user));
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
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
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                IUser user = null;
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null)
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
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
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
                ISettings settings = _settingsFactory.CreateCore(_settings.Value);
                IUser user = null;
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");                
                if (result == null)
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
                    if (roles.Any(r => string.Equals(r, "sysadmin", StringComparison.OrdinalIgnoreCase)))
                        user.Roles = user.Roles | UserRole.SystemAdministrator;
                    else
                        user.Roles = user.Roles & (~UserRole.SystemAdministrator);
                    if (roles.Any(r => string.Equals(r, "actadmin", StringComparison.OrdinalIgnoreCase)))
                        user.Roles = user.Roles | UserRole.AccountAdministrator;
                    else
                        user.Roles = user.Roles & (~UserRole.AccountAdministrator);
                    await _userSaver.Update(settings, user);
                    result = Ok();
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
