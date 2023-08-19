using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccountAPI
{
    public abstract class AccountControllerBase : CommonControllerBase
    {
        protected readonly IOptions<Settings> _settings;
        protected readonly SettingsFactory _settingsFactory;
        protected readonly IExceptionService _exceptionService;
        private readonly MapperFactory _mapperFactory;
        private BrassLoon.Interface.Log.ISettings _loggSettings;
        private BrassLoon.Interface.Account.ISettings _accountSettings;
        private CoreSettings _coreSettings;

        protected AccountControllerBase(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
            _exceptionService = exceptionService;
        }

        [NonAction]
        protected async Task<IUser> GetUser(IUserFactory userFactory, BrassLoon.CommonCore.ISettings settings)
        {
            string referenceId = GetCurrentUserReferenceId();
            return await userFactory.GetByReferenceId(settings, referenceId);
        }

        [NonAction]
        protected override bool UserCanAccessAccount(Guid accountId)
        {
            bool result = base.UserCanAccessAccount(accountId);
            if (!result)
            {
                result = User.Claims.Any(
                    c => string.Equals(ClaimTypes.Role, c.Type, StringComparison.OrdinalIgnoreCase) && string.Equals("actadmin", c.Value, StringComparison.OrdinalIgnoreCase)
                    );
            }
            return result;
        }

        [NonAction]
        protected IMapper CreateMapper() => _mapperFactory.Create();

        [NonAction]
        protected BrassLoon.Interface.Log.ISettings CreateLogSettings() => CreateLogSettings(_settings.Value, GetAccessToken());

        [NonAction]
        protected override BrassLoon.Interface.Log.ISettings CreateLogSettings(CommonApiSettings settings, string accessToken)
        {
            if (_loggSettings == null)
                _loggSettings = _settingsFactory.CreateLog(settings, accessToken);
            return _loggSettings;
        }

        [NonAction]
        protected override BrassLoon.Interface.Account.ISettings CreateAccountSettings(CommonApiSettings settings, string accessToken) => throw new NotImplementedException();

        [NonAction]
        protected CoreSettings CreateCoreSettings()
        {
            if (_coreSettings == null)
                _coreSettings = _settingsFactory.CreateCore(_settings.Value);
            return _coreSettings;
        }

        [NonAction]
        protected Task LogException(Exception ex) => LogException(ex, _exceptionService, _settings.Value);
    }
}
