using Autofac;
using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Account.Framework.Enumerations;
using BrassLoon.Interface.Account.Models;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInvitationController : AccountControllerBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly IContainer _container;

        public UserInvitationController(IOptions<Settings> settings,
            IContainer container)
        {
            _settings = settings;
            _container = container;
        }

        [NonAction]
        private async Task<UserInvitation> Map(IMapper mapper, CoreSettings settings, IUserInvitation innerInvitation)
        {
            UserInvitation result = mapper.Map<UserInvitation>(innerInvitation);
            result.EmailAddress = (await innerInvitation.GetEmailAddress(settings)).Address;
            return result;
        }

        [HttpGet("/api/Account/{accountId}/Invitation")]
        [ProducesResponseType(typeof(UserInvitation[]), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> GetByAccountId([FromRoute] Guid? accountId)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!accountId.HasValue || accountId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing account id parameter value");
                if (result == null && !UserCanAccessAccount(accountId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                    IUserInvitationFactory factory = scope.Resolve<IUserInvitationFactory>();
                    IEnumerable<IUserInvitation> innerInvitations = await factory.GetByAccountId(settings, accountId.Value);
                    IMapper mapper = MapperConfigurationFactory.CreateMapper();
                    result = Ok(
                        await Task.WhenAll(
                            innerInvitations.Select<IUserInvitation, Task<UserInvitation>>(innerInvitation => Map(mapper, settings, innerInvitation))
                            ));
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

        [NonAction]
        private async Task<bool> UserCanAccessInvitation(CoreSettings settings, IUserInvitation userInvitation)
        {
            bool result = UserCanAccessAccount(userInvitation.AccountId);
            if (!result)
            {
                string email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
                IEmailAddress emailAddress = await userInvitation.GetEmailAddress(settings);
                result = string.Equals(email, emailAddress.Address, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserInvitation), 200)]
        [Authorize("READ:ACCOUNT")]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing invitation id parameter value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                    IUserInvitationFactory factory = scope.Resolve<IUserInvitationFactory>();
                    IUserInvitation innerInvitation = await factory.Get(settings, id.Value);
                    if (innerInvitation != null && !(await UserCanAccessInvitation(settings, innerInvitation)))
                        innerInvitation = null;
                    if (innerInvitation == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        result = Ok(
                            await Map(mapper, settings, innerInvitation)
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

        [NonAction]
        private async Task<IEmailAddress> GetEmailAddress(CoreSettings settings, 
            IEmailAddressFactory emailAddressFactory, 
            IEmailAddressSaver emailAddressSaver, 
            string address)
        {
            IEmailAddress result = await emailAddressFactory.GetByAddress(settings, address);
            if (result == null)
            {
                result = emailAddressFactory.Create(address);
                await emailAddressSaver.Create(settings, result);
            }    
            return result;
        }

        [HttpPost("/api/Account/{accountId}/Invitation")]
        [ProducesResponseType(typeof(UserInvitation), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Create([FromRoute] Guid? accountId, [FromBody] UserInvitation userInvitation)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!accountId.HasValue || accountId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing account id parameter value");
                if (result == null && !UserCanAccessAccount(accountId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null && userInvitation == null)
                    result = BadRequest("Missing user invitation body");
                if (result == null && string.IsNullOrEmpty(userInvitation.EmailAddress))
                    result = BadRequest("Missing user email address value");
                if (result == null && !userInvitation.ExpirationTimestamp.HasValue)
                    userInvitation.ExpirationTimestamp = DateTime.UtcNow.AddDays(7);
                if (result == null && userInvitation.ExpirationTimestamp.HasValue && userInvitation.ExpirationTimestamp.Value.ToUniversalTime() <= DateTime.UtcNow)
                    result = BadRequest("Invalid expiration timestamp in the past");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                    IAccountFactory accountFactory = scope.Resolve<IAccountFactory>();
                    IAccount account = await accountFactory.Get(settings, accountId.Value);
                    if (account == null)
                        result = NotFound();
                    else
                    {
                        IEmailAddress emailAddress = await GetEmailAddress(
                            settings,
                            scope.Resolve<IEmailAddressFactory>(),
                            scope.Resolve<IEmailAddressSaver>(),
                            userInvitation.EmailAddress
                            );
                        IUserInvitationFactory invitationFactory = scope.Resolve<IUserInvitationFactory>();
                        IUserInvitation innerInvitation = invitationFactory.Create(account, emailAddress);
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        mapper.Map<UserInvitation, IUserInvitation>(userInvitation, innerInvitation);
                        innerInvitation.Status = UserInvitationStatus.Created;
                        IUserInvitationSaver saver = scope.Resolve<IUserInvitationSaver>();
                        await saver.Create(settings, innerInvitation);
                        result = Ok(await Map(mapper, settings, innerInvitation));
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
        [ProducesResponseType(typeof(UserInvitation), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] UserInvitation userInvitation)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && userInvitation == null)
                    result = BadRequest("Missing user invitation body");
                if (result == null && !userInvitation.ExpirationTimestamp.HasValue)
                    result = BadRequest("Missing expiration timestamp value");
                if (result == null && !userInvitation.Status.HasValue)
                    result = BadRequest("Missing status value");
                if (result == null)
                {
                    using ILifetimeScope scope = _container.BeginLifetimeScope();
                    SettingsFactory settingsFactory = scope.Resolve<SettingsFactory>();
                    CoreSettings settings = settingsFactory.CreateAccount(_settings.Value);
                    IAccountFactory accountFactory = scope.Resolve<IAccountFactory>();
                    IUserInvitationFactory invitationFactory = scope.Resolve<IUserInvitationFactory>();
                    IUserInvitation innerInvitation = await invitationFactory.Get(settings, id.Value);
                    if (innerInvitation != null && !(await UserCanAccessInvitation(settings, innerInvitation)))
                        innerInvitation = null;
                    if (innerInvitation == null)
                        result = NotFound();
                    else
                    {
                        if (DateTime.UtcNow < innerInvitation.ExpirationTimestamp && 
                            innerInvitation.Status != UserInvitationStatus.Cancelled && 
                            userInvitation.Status == (short)UserInvitationStatus.Completed &&
                            !UserTokenHasAccount(innerInvitation.AccountId))
                        {
                            await AddAccountUser(scope.Resolve<IUserFactory>(), scope.Resolve<IAccountSaver>(), settings, innerInvitation.AccountId);
                        }
                        IMapper mapper = MapperConfigurationFactory.CreateMapper();
                        mapper.Map<UserInvitation, IUserInvitation>(userInvitation, innerInvitation);
                        IUserInvitationSaver saver = scope.Resolve<IUserInvitationSaver>();
                        await saver.Update(settings, innerInvitation);
                        result = Ok(await Map(mapper, settings, innerInvitation));
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

        [NonAction]
        protected bool UserTokenHasAccount(Guid accountId)
        {
            string[] accountIds = Regex.Split(User.Claims.First(c => c.Type == "accounts").Value, @"\s+", RegexOptions.IgnoreCase);
            return accountIds.Where(id => !string.IsNullOrEmpty(id)).Any(id => Guid.Parse(id).Equals(accountId));
        }

        [NonAction]
        private async Task AddAccountUser(IUserFactory userFactory, IAccountSaver accountSaver, CoreSettings settings, Guid accountId)
        {
            IUser user = await GetUser(userFactory, settings);
            await accountSaver.AddUser(settings, user.UserId, accountId);
        }
    }
}
