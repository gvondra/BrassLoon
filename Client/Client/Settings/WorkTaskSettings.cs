using BrassLoon.Interface.WorkTask;
using System.Threading.Tasks;

namespace BrassLoon.Client.Settings
{
    public class WorkTaskSettings : ISettings
    {
        public string BaseAddress { get; set; }

        public Task<string> GetToken()
        {
            return Task.FromResult(AccessToken.Get.Token);
        }
    }
}
