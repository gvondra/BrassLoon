using System.Threading.Tasks;

namespace BrassLoon.Address.Framework
{
    public interface IAddressFactory
    {
        IAddress Create(Guid domainId);
        Task<IAddress> Get(ISettings settings, Guid domainId, Guid id);
    }
}
