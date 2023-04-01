using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public interface ISettings
    {
        string BaseAddress { get; }

        Task<string> GetToken();
    }
}
