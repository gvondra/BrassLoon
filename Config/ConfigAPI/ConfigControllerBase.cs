using BrassLoon.CommonAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigAPI
{
    public abstract class ConfigControllerBase : CommonControllerBase
    {
        protected readonly IOptions<Settings> _settings;
        protected readonly SettingsFactory _settingsFactory;
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
        protected override BrassLoon.Interface.Account.ISettings CreateAccountSettings(CommonApiSettings settings, string accessToken)
        {
            if (_accountSettings == null)
                _accountSettings = _settingsFactory.CreateAccount(settings, accessToken);
            return _accountSettings;
        }
    }
}
