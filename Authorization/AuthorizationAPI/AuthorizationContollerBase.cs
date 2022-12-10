using BrassLoon.CommonAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthorizationAPI
{
    public abstract class AuthorizationContollerBase : CommonControllerBase
    {
        protected readonly IOptions<Settings> _settings;
        protected readonly SettingsFactory _settingsFactory;
        private BrassLoon.Interface.Log.ISettings _loggSettings;
        private BrassLoon.Interface.Account.ISettings _accountSettings;

        protected AuthorizationContollerBase(IOptions<Settings> settings, SettingsFactory settingsFactory)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
        }

        [NonAction]
        protected override BrassLoon.Interface.Log.ISettings CreateLogSettings(CommonApiSettings settings, string accessToken)
        {
            if (_loggSettings == null)
                _loggSettings = _settingsFactory.CreateLog(settings, accessToken);
            return _loggSettings;
        }

        [NonAction]
        protected override BrassLoon.Interface.Account.ISettings CreateAccountSettings(CommonApiSettings settings, string accessToken)
        {
            if (_accountSettings == null)
                _accountSettings = _settingsFactory.CreateAccount(settings, accessToken);
            return _accountSettings;
        }
    }
}
