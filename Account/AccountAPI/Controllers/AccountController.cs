using Autofac;
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
        private readonly IContainer _container;

        public AccountController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
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
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        public async Task<IActionResult> GetAll()
        {
            IActionResult result = null;
            using (ILifetimeScope scope = _container.BeginLifetimeScope())
            {
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                IUser user = await GetUser(scope.Resolve<IUserFactory>(), settings);
                IAccountFactory accountFactory = scope.Resolve<IAccountFactory>();
                IEnumerable<IAccount> accounts = await accountFactory.GetByUserId(settings, user.UserId);
                IMapper mapper = MapperConfigurationFactory.CreateMapper();
                result = Ok(
                    accounts.Select<IAccount, Account>(innerAccount => mapper.Map<Account>(innerAccount))
                    );
            }
            return result;
        }

        [NonAction]
        public async Task<IActionResult> GetByEmailAddress(string emailAddress)
        {
            IActionResult result = null;
            using (ILifetimeScope scope = _container.BeginLifetimeScope())
            {
                SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                IUserFactory userFactory = scope.Resolve<IUserFactory>();
                IEnumerable<IUser> users = await userFactory.GetByEmailAddress(settings, emailAddress);                
                IAccountFactory accountFactory = scope.Resolve<IAccountFactory>();
                ConcurrentBag<Task<IEnumerable<IAccount>>> accounts = new ConcurrentBag<Task<IEnumerable<IAccount>>>();                
                users.AsParallel().ForAll(user => accounts.Add(accountFactory.GetByUserId(settings, user.UserId)));
                IMapper mapper = MapperConfigurationFactory.CreateMapper();
                result = Ok(
                    (await Task.WhenAll<IEnumerable<IAccount>>(accounts))
                    .SelectMany(results => results)
                    .Where(a => UserCanAccessAccount(a.AccountId))
                    .Select<IAccount, Account>(innerAccount => mapper.Map<Account>(innerAccount))
                    .ToList()
                    );
            }
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
                    using (ILifetimeScope scope = _container.BeginLifetimeScope())
                    {
                        SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                        CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                        IAccountFactory accountFactory = scope.Resolve<IAccountFactory>();
                        IAccount account = await accountFactory.Get(settings, id);
                        if (account == null)
                            result = NotFound();
                        else
                        {
                            IMapper mapper = MapperConfigurationFactory.CreateMapper();
                            result = Ok(mapper.Map<Account>(account));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }
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
                    using (ILifetimeScope scope = _container.BeginLifetimeScope())
                    {
                        SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                        CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                        IUser user = await GetUser(scope.Resolve<IUserFactory>(), settings);
                        IAccountFactory accountFactory = scope.Resolve<IAccountFactory>();
                        IAccount innerAccount = accountFactory.Create();
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        mapper.Map<Account, IAccount>(account, innerAccount);
                        IAccountSaver saver = scope.Resolve<IAccountSaver>();
                        await saver.Create(settings, user.UserId, innerAccount);
                        result = Ok(
                            mapper.Map<Account>(innerAccount)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }
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
                    using (ILifetimeScope scope = _container.BeginLifetimeScope())
                    {
                        SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                        CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                        IAccountFactory accountFactory = scope.Resolve<IAccountFactory>();
                        IAccount innerAccount = await accountFactory.Get(settings, id);
                        if (innerAccount == null)
                            result = NotFound();
                        else
                        {
                            IMapper mapper = MapperConfigurationFactory.CreateMapper();
                            mapper.Map<Account, IAccount>(account, innerAccount);
                            IAccountSaver saver = scope.Resolve<IAccountSaver>();
                            await saver.Update(settings, innerAccount);
                            result = Ok(
                                mapper.Map<Account>(innerAccount)
                                );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }
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
                    using (ILifetimeScope scope = _container.BeginLifetimeScope())
                    {
                        SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                        CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                        IAccountSaver accountSaver = scope.Resolve<IAccountSaver>();
                        await accountSaver.RemoveUser(settings, userId.Value, accountId.Value);
                        result = Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                using (ILifetimeScope scope = _container.BeginLifetimeScope())
                {
                    await LogException(ex, scope.Resolve<IExceptionService>(), scope.Resolve<SettingsFactory>(), _settings.Value);
                }
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
