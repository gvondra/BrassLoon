using BrassLoon.CommonAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConfigAPI
{
    public abstract class ConfigControllerBase : CommonControllerBase
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Fields should be private
        protected readonly IOptions<Settings> _settings;
        protected readonly SettingsFactory _settingsFactory;
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore SA1401 // Fields should be private
        private BrassLoon.Interface.Log.ISettings _loggSettings;
        private BrassLoon.Interface.Account.ISettings _accountSettings;

        protected ConfigControllerBase(IOptions<Settings> settings, SettingsFactory settingsFactory)
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
        protected override BrassLoon.Interface.Log.ISettings CreateLogSettings(CommonApiSettings settings) => _settingsFactory.CreateLog(settings);

        [NonAction]
        protected override BrassLoon.Interface.Account.ISettings CreateAccountSettings(CommonApiSettings settings, string accessToken)
        {
            if (_accountSettings == null)
                _accountSettings = _settingsFactory.CreateAccount(settings, accessToken);
            return _accountSettings;
        }
    }
}
