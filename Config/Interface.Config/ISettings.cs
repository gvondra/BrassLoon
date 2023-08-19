using System.Threading.Tasks;

namespace BrassLoon.Interface.Config
{
    public interface ISettings
    {
        string BaseAddress { get; }

        Task<string> GetToken();
    }
}
