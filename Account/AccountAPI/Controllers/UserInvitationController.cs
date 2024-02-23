using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Account.Framework.Enumerations;
using BrassLoon.Interface.Account.Models;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UserInvitationController> _logger;
        private readonly IAccountFactory _accountFactory;
        private readonly IAccountSaver _accountSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly IEmailAddressSaver _emailAddressSaver;
        private readonly IUserFactory _userFactory;
        private readonly IUserInvitationFactory _userInvitationFactory;
        private readonly IUserInvitationSaver _userInvitationSaver;

        public UserInvitationController(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<UserInvitationController> logger,
            MapperFactory mapperFactory,
            IAccountFactory accountFactory,
            IAccountSaver accountSaver,
            IEmailAddressFactory emailAddressFactory,
            IEmailAddressSaver emailAddressSaver,
            IUserFactory userFactory,
            IUserInvitationFactory userInvitationFactory,
            IUserInvitationSaver userInvitationSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory)
        {
            _logger = logger;
            _accountFactory = accountFactory;
            _accountSaver = accountSaver;
            _emailAddressFactory = emailAddressFactory;
            _emailAddressSaver = emailAddressSaver;
            _userFactory = userFactory;
            _userInvitationFactory = userInvitationFactory;
            _userInvitationSaver = userInvitationSaver;
        }

        [NonAction]
        private static async Task<UserInvitation> Map(IMapper mapper, CoreSettings settings, IUserInvitation innerInvitation)
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
                if (!accountId.HasValue || accountId.Value.Equals(Guid.Empty))
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
                    IEnumerable<IUserInvitation> innerInvitations = await _userInvitationFactory.GetByAccountId(settings, accountId.Value);
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        await Task.WhenAll(
                            innerInvitations.Select(innerInvitation => Map(mapper, settings, innerInvitation))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing invitation id parameter value");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IUserInvitation innerInvitation = await _userInvitationFactory.Get(settings, id.Value);
                    if (innerInvitation != null && !await UserCanAccessInvitation(settings, innerInvitation))
                        innerInvitation = null;
                    if (innerInvitation == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            await Map(mapper, settings, innerInvitation));
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

        [NonAction]
        private static async Task<IEmailAddress> GetEmailAddress(
            CoreSettings settings,
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
                if (!accountId.HasValue || accountId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing account id parameter value");
                }
                else if (!UserCanAccessAccount(accountId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else if (userInvitation == null)
                {
                    result = BadRequest("Missing user invitation body");
                }
                else if (string.IsNullOrEmpty(userInvitation.EmailAddress))
                {
                    result = BadRequest("Missing user email address value");
                }
                else if (!userInvitation.ExpirationTimestamp.HasValue)
                {
                    userInvitation.ExpirationTimestamp = DateTime.UtcNow.AddDays(7);
                }
                else if (userInvitation.ExpirationTimestamp.HasValue && userInvitation.ExpirationTimestamp.Value.ToUniversalTime() <= DateTime.UtcNow)
                {
                    result = BadRequest("Invalid expiration timestamp in the past");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IAccount account = await _accountFactory.Get(settings, accountId.Value);
                    if (account == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IEmailAddress emailAddress = await GetEmailAddress(
                            settings,
                            _emailAddressFactory,
                            _emailAddressSaver,
                            userInvitation.EmailAddress);
                        IUserInvitation innerInvitation = _userInvitationFactory.Create(account, emailAddress);
                        IMapper mapper = CreateMapper();
                        _ = mapper.Map(userInvitation, innerInvitation);
                        innerInvitation.Status = UserInvitationStatus.Created;
                        await _userInvitationSaver.Create(settings, innerInvitation);
                        result = Ok(await Map(mapper, settings, innerInvitation));
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

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserInvitation), 200)]
        [Authorize("EDIT:ACCOUNT")]
        public async Task<IActionResult> Update([FromRoute] Guid? id, [FromBody] UserInvitation userInvitation)
        {
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else if (userInvitation == null)
                {
                    result = BadRequest("Missing user invitation body");
                }
                else if (!userInvitation.ExpirationTimestamp.HasValue)
                {
                    result = BadRequest("Missing expiration timestamp value");
                }
                else if (!userInvitation.Status.HasValue)
                {
                    result = BadRequest("Missing status value");
                }
                else
                {
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IUserInvitation innerInvitation = await _userInvitationFactory.Get(settings, id.Value);
                    if (innerInvitation != null && !await UserCanAccessInvitation(settings, innerInvitation))
                        innerInvitation = null;
                    if (innerInvitation == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        if (DateTime.UtcNow < innerInvitation.ExpirationTimestamp &&
                            innerInvitation.Status != UserInvitationStatus.Cancelled &&
                            userInvitation.Status == (short)UserInvitationStatus.Completed &&
                            !UserTokenHasAccount(innerInvitation.AccountId))
                        {
                            await AddAccountUser(_userFactory, _accountSaver, settings, innerInvitation.AccountId);
                        }
                        IMapper mapper = CreateMapper();
                        _ = mapper.Map(userInvitation, innerInvitation);
                        await _userInvitationSaver.Update(settings, innerInvitation);
                        result = Ok(await Map(mapper, settings, innerInvitation));
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
