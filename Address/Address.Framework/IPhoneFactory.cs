using System.Threading.Tasks;

namespace BrassLoon.Address.Framework
{
    public interface IPhoneFactory
    {
        IPhone Create(Guid domainId);
        Task<IPhone> Get(ISettings settings, Guid domainId, Guid id);
    }
}
