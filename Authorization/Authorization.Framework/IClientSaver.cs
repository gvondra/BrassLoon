using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IClientSaver
    {
        Task Create(ISettings settings, IClient client, IEmailAddress userEmailAddress = null);
        Task Update(ISettings settings, IClient client, IEmailAddress userEmailAddress = null);
    }
}
