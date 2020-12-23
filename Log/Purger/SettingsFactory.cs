using AccountInterface = BrassLoon.Interface.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Purger
{
    public sealed class SettingsFactory
    {
        private readonly AccountInterface.ITokenService _tokenService;

        public SettingsFactory(AccountInterface.ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public CoreSettings CreateCore(Settings settings)
        {
            return new CoreSettings(settings);
        }

        public AccountSettings CreateAccount(Settings settings)
        {
            return new AccountSettings(settings, _tokenService);
        }

        public LogSettings CreateLog(Settings settings)
        {
            return new LogSettings(settings, _tokenService, this);
        }
    }
}
