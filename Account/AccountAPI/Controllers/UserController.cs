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

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : AccountControllerBase
    {
        private readonly Lazy<Log.IExceptionService> _exceptionService;
        private readonly IUserFactory _userFactory;

        public UserController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Lazy<Log.IExceptionService> exceptionService,
            IUserFactory userFactory)            
            : base(settings, settingsFactory) 
        {
            _exceptionService = exceptionService;
            _userFactory = userFactory;
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
                    ISettings settings = _settingsFactory.CreateAccount(_settings.Value);
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
    }
}
