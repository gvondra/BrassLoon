using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public interface ISettings
    {
        string BaseAddress { get; }

        Task<string> GetToken();
    }
}
