using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Interface.Account.Models;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : AccountControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountFactory _accountFactory;
        private readonly IAccountSaver _accountSaver;
        private readonly IUserFactory _userFactory;

        public AccountController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<AccountController> logger,
            MapperFactory mapperFactory,
            IAccountFactory accountFactory,
            IAccountSaver accountSaver,
            IUserFactory userFactory)
            : base(settings, settingsFactory, exceptionService, mapperFactory)
        {
            _logger = logger;
            _accountFactory = accountFactory;
            _accountSaver = accountSaver;
            _userFactory = userFactory;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(Account[]), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> Search([FromQuery] string emailAddress)
        {
            IActionResult result;
            try
            {
                if (!string.IsNullOrEmpty(emailAddress))
                    result = await GetByEmailAddress(emailAddress);
                else
                    result = await GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        public async Task<IActionResult> GetAll()
        {
            IActionResult result = null;
            CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
            IUser user = await GetUser(_userFactory, settings);
            IEnumerable<IAccount> accounts = await _accountFactory.GetByUserId(settings, user.UserId);
            IMapper mapper = CreateMapper();
            result = Ok(
                accounts.Select(mapper.Map<Account>)
                );
            return result;
        }

        [NonAction]
        public async Task<IActionResult> GetByEmailAddress(string emailAddress)
        {
            IActionResult result = null;
            CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
            IEnumerable<IUser> users = await _userFactory.GetByEmailAddress(settings, emailAddress);
            ConcurrentBag<Task<IEnumerable<IAccount>>> accounts = new ConcurrentBag<Task<IEnumerable<IAccount>>>();
            users.AsParallel().ForAll(user => accounts.Add(_accountFactory.GetByUserId(settings, user.UserId)));
            IMapper mapper = CreateMapper();
            result = Ok(
                (await Task.WhenAll(accounts))
                .SelectMany(results => results)
                .Where(a => UserCanAccessAccount(a.AccountId))
                .Select(mapper.Map<Account>)
                .ToList()
                );
            return result;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Account), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> Get(Guid id)
        {
            IActionResult result;
            try
            {
                if (Guid.Empty.Equals(id))
                {
                    result = BadRequest("Invalid account id");
                }
                else if (!UserCanAccessAccount(id))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IAccount account = await _accountFactory.Get(settings, id);
                    if (account == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(mapper.Map<Account>(account));
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

        [HttpPost()]
        [ProducesResponseType(typeof(Account), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Create([FromBody] Account account)
        {
            IActionResult result;
            try
            {
                if (account == null)
                {
                    result = BadRequest("Missing account data");
                }
                else if (string.IsNullOrEmpty(account.Name))
                {
                    result = BadRequest("Missing account name");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IUser user = await GetUser(_userFactory, settings);
                    IAccount innerAccount = _accountFactory.Create();
                    IMapper mapper = CreateMapper();
                    _ = mapper.Map(account, innerAccount);
                    await _accountSaver.Create(settings, user.UserId, innerAccount);
                    result = Ok(
                        mapper.Map<Account>(innerAccount)
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

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Account), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Account account)
        {
            IActionResult result;
            try
            {
                if (account == null)
                {
                    result = BadRequest("Missing account data");
                }
                else if (string.IsNullOrEmpty(account.Name))
                {
                    result = BadRequest("Missing account name");
                }
                else if (Guid.Empty.Equals(id))
                {
                    result = BadRequest("Invalid account id");
                }
                else if (!UserCanAccessAccount(id))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IAccount innerAccount = await _accountFactory.Get(settings, id);
                    if (innerAccount == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        _ = mapper.Map(account, innerAccount);
                        await _accountSaver.Update(settings, innerAccount);
                        result = Ok(
                            mapper.Map<Account>(innerAccount)
                            );
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

        [HttpPatch("{id}/Locked")]
        [Authorize("ADMIN:ACCOUNT")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Dictionary<string, string> data)
        {
            IActionResult result;
            bool locked;
            try
            {
                if (data == null)
                {
                    result = BadRequest("Missing patch data");
                }
                else if (!data.ContainsKey("Locked") || string.IsNullOrEmpty(data["Locked"]))
                {
                    result = BadRequest("Missing Locked value");
                }
                else if (Guid.Empty.Equals(id))
                {
                    result = BadRequest("Invalid account id");
                }
                else if (!UserCanAccessAccount(id))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else if (!bool.TryParse(data["Locked"], out locked))
                {
                    result = BadRequest("Invalid locked value.  Expecting 'True' or 'False'");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IAccount innerAccount = await _accountFactory.Get(settings, id);
                    if (innerAccount == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        await _accountSaver.UpdateLocked(settings, innerAccount.AccountId, locked);
                        result = Ok();
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

        [HttpDelete("{accountId}/User/{userId}")]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid? accountId, [FromRoute] Guid? userId)
        {
            IActionResult result;
            try
            {
                if (!accountId.HasValue || Guid.Empty.Equals(accountId.Value))
                {
                    result = BadRequest("Missing account id parameter value");
                }
                else if (!userId.HasValue || Guid.Empty.Equals(userId.Value))
                {
                    result = BadRequest("Missing user id parameter value");
                }
                else if (!UserCanAccessAccount(accountId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    await _accountSaver.RemoveUser(settings, userId.Value, accountId.Value);
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

        [HttpGet("{accountId}/User")]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> GetUsers([FromRoute] Guid? accountId)
        {
            IActionResult result = null;
            try
            {
                if (!accountId.HasValue || Guid.Empty.Equals(accountId.Value))
                {
                    result = BadRequest("Missing account id parameter value");
                }
                else if (!UserCanAccessAccount(accountId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IEnumerable<IUser> innerUsers = await _userFactory.GetByAccountId(settings, accountId.Value);
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        innerUsers.Select(mapper.Map<User>)
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
    }
}
