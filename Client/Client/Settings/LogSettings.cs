using BrassLoon.Interface.Log;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Client.Settings
{
    public class LogSettings : ISettings
    {
        public string BaseAddress { get; set; }

        public Task<string> GetToken()
        {
            return Task.FromResult(AccessToken.Get.Token);
        }
    }
}
