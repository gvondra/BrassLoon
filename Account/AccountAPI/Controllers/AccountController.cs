using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Interface.Account.Models;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IOptions<Settings> _settings;
        private readonly SettingsFactory _settingsFactory;
        private readonly Lazy<IExceptionService> _exceptionService;
        private readonly IAccountFactory _accountFactory;
        private readonly IAccountSaver _accountSaver;
        private readonly IUserFactory _userFactory;

        public AccountController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Lazy<IExceptionService> exceptionService,
            IAccountFactory accountFactory,
            IAccountSaver accountSaver,
            IUserFactory userFactory)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
            _exceptionService = exceptionService;
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
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        public async Task<IActionResult> GetAll()
        {
            IActionResult result = null;
            CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
            IUser user = await GetUser(_userFactory, settings);
            IEnumerable<IAccount> accounts = await _accountFactory.GetByUserId(settings, user.UserId);
            IMapper mapper = MapperConfigurationFactory.CreateMapper();
            result = Ok(
                accounts.Select<IAccount, Account>(innerAccount => mapper.Map<Account>(innerAccount))
                );
            return result;
        }

        [NonAction]
        public async Task<IActionResult> GetByEmailAddress(string emailAddress)
        {
            IActionResult result = null;
            CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
            IEnumerable<IUser> users = await _userFactory.GetByEmailAddress(settings, emailAddress);
            ConcurrentBag<Task<IEnumerable<IAccount>>> accounts = new ConcurrentBag<Task<IEnumerable<IAccount>>>();
            users.AsParallel().ForAll(user => accounts.Add(_accountFactory.GetByUserId(settings, user.UserId)));
            IMapper mapper = MapperConfigurationFactory.CreateMapper();
            result = Ok(
                (await Task.WhenAll<IEnumerable<IAccount>>(accounts))
                .SelectMany(results => results)
                .Where(a => UserCanAccessAccount(a.AccountId))
                .Select<IAccount, Account>(innerAccount => mapper.Map<Account>(innerAccount))
                .ToList()
                );
            return result;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Account), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> Get(Guid id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && Guid.Empty.Equals(id))
                    result = BadRequest("Invalid account id");
                if (result == null && !UserCanAccessAccount(id))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
                    IAccount account = await _accountFactory.Get(settings, id);
                    if (account == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(mapper.Map<Account>(account));
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(Account), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Create([FromBody] Account account)
        {
            IActionResult result = null;
            try
            {
                if (result == null && account == null)
                    result = BadRequest("Missing account data");
                if (result == null && string.IsNullOrEmpty(account.Name))
                    result = BadRequest("Missing account name");
                if (result == null)
                {
                    CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
                    IUser user = await GetUser(_userFactory, settings);
                    IAccount innerAccount = _accountFactory.Create();
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    mapper.Map<Account, IAccount>(account, innerAccount);
                    await _accountSaver.Create(settings, user.UserId, innerAccount);
                    result = Ok(
                        mapper.Map<Account>(innerAccount)
                        );
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Account), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Account account)
        {
            IActionResult result = null;
            try
            {
                if (result == null && account == null)
                    result = BadRequest("Missing account data");
                if (result == null && string.IsNullOrEmpty(account.Name))
                    result = BadRequest("Missing account name");
                if (result == null && Guid.Empty.Equals(id))
                    result = BadRequest("Invalid account id");
                if (result == null && !UserCanAccessAccount(id))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
                    IAccount innerAccount = await _accountFactory.Get(settings, id);
                    if (innerAccount == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        mapper.Map<Account, IAccount>(account, innerAccount);
                        await _accountSaver.Update(settings, innerAccount);
                        result = Ok(
                            mapper.Map<Account>(innerAccount)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPatch("{id}/Locked")]
        [Authorize("ADMIN:ACCOUNT")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Dictionary<string, string> data)
        {            
            IActionResult result = null;
            bool locked = default;
            try
            {
                if (result == null && data == null)
                    result = BadRequest("Missing patch data");
                if (result == null && (!data.ContainsKey("Locked") || string.IsNullOrEmpty(data["Locked"])))
                    result = BadRequest("Missing Locked value");
                if (result == null && Guid.Empty.Equals(id))
                    result = BadRequest("Invalid account id");
                if (result == null && !UserCanAccessAccount(id))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null && !bool.TryParse(data["Locked"], out locked))
                    result = BadRequest("Invalid locked value.  Expecting 'True' or 'False'");
                if (result == null)
                {
                    CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
                    IAccount innerAccount = await _accountFactory.Get(settings, id);
                    if (innerAccount == null)
                        result = NotFound();
                    else
                    {
                        await _accountSaver.UpdateLocked(settings, innerAccount.AccountId, locked);
                        result = Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpDelete("{accountId}/User/{userId}")]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid? accountId, [FromRoute] Guid? userId)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!accountId.HasValue || Guid.Empty.Equals(accountId.Value)))
                    result = BadRequest("Missing account id parameter value");
                if (result == null && (!userId.HasValue || Guid.Empty.Equals(userId.Value)))
                    result = BadRequest("Missing user id parameter value");
                if (result == null && !UserCanAccessAccount(accountId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
                    await _accountSaver.RemoveUser(settings, userId.Value, accountId.Value);
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

        [HttpGet("{accountId}/User")]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> GetUsers([FromRoute] Guid? accountId)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!accountId.HasValue || Guid.Empty.Equals(accountId.Value)))
                    result = BadRequest("Missing account id parameter value");
                if (result == null && !UserCanAccessAccount(accountId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {         
                    CoreSettings settings = _settingsFactory.CreateAccount(_settings.Value);
                    IEnumerable<IUser> innerUsers = await _userFactory.GetByAccountId(settings, accountId.Value);
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    result = Ok(
                        innerUsers.Select<IUser, User>(innerUser => mapper.Map<User>(innerUser))
                        );
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
