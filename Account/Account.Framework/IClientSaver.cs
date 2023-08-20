using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IClientSaver
    {
        Task Create(ISettings settings, IClient client);
        Task Update(ISettings settings, IClient client);
        Task Update(ISettings settings, IClient client, string secret);
    }
}
