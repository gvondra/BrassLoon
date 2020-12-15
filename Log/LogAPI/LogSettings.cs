using BrassLoon.Interface.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAPI
{
    public class LogSettings : ISettings
    {
        private string _accessToken;

        public LogSettings(string accessToken)
        {
            _accessToken = accessToken;
        }

        public string BaseAddress { get; set; }

        public Task<string> GetToken()
        {
            return Task.FromResult(_accessToken);
        }
    }
}
