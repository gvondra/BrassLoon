using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public interface ISettings
    {
        string BaseAddress { get; }

        Task<string> GetToken();
    }
}
