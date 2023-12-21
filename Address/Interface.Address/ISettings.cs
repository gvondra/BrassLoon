using System.Threading.Tasks;

namespace BrassLoon.Interface.Address
{
    public interface ISettings
    {
        string BaseAddress { get; }

        Task<string> GetToken();
    }
}
