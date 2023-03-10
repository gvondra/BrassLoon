﻿using BrassLoon.Interface.Log;
using System.Threading.Tasks;

namespace BrassLoon.CommonAPI
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
