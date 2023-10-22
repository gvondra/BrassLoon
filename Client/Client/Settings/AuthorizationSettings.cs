using BrassLoon.Interface.Authorization;
using System.Threading.Tasks;

namespace BrassLoon.Client.Settings
{
    public class AuthorizationSettings : ISettings
    {
        public string BaseAddress { get; set; }

        public Task<string> GetToken()
        {
            return Task.FromResult(AccessToken.Get.Token);
        }
    }
}
