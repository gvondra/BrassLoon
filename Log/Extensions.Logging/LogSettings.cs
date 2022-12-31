using BrassLoon.Interface.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Extensions.Logging
{
    public class LogSettings : ISettings
    {
        private readonly LoggerConfiguration _loggerConfiguration;
        private readonly Account.ITokenService _tokenService;
        private string _token;

        public LogSettings(Account.ITokenService tokenService, LoggerConfiguration loggerConfiguration)
        {
            _loggerConfiguration = loggerConfiguration;
            _tokenService = tokenService;
        }

        public string BaseAddress => _loggerConfiguration.LogApiBaseAddress;

        public async Task<string> GetToken()
        {
            if (string.IsNullOrEmpty(_token))
                _token = await _tokenService.CreateClientCredentialToken(new AccountSettings(_loggerConfiguration), _loggerConfiguration.LogClientId, _loggerConfiguration.LogClientSecret);
            return _token;
        }
    }
}
