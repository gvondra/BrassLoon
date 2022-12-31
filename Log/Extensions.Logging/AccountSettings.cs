using BrassLoon.Interface.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Extensions.Logging
{
    internal class AccountSettings : ISettings
    {
        private readonly LoggerConfiguration _loggerConfiguration;

        public AccountSettings(LoggerConfiguration loggerConfiguration)
        {
            _loggerConfiguration = loggerConfiguration;
        }

        public string BaseAddress => _loggerConfiguration.AccountApiBaseAddress;

        public Task<string> GetToken()
        {
            throw new NotSupportedException("this component only makes 1 call and it doesn't need this token");
        }
    }
}
