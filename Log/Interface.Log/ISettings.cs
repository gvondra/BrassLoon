using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public interface ISettings
    {
        string BaseAddress { get; }

        Task<string> GetToken();
    }
}
