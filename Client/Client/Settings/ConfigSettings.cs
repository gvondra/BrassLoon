using BrassLoon.Interface.Config;
using System.Threading.Tasks;

namespace BrassLoon.Client.Settings
{
    public class ConfigSettings : ISettings
    {
        public string BaseAddress { get; set; }

        public Task<string> GetToken()
        {
            return Task.FromResult(AccessToken.Get.Token);
        }
    }
}
