using System.Threading.Tasks;

namespace BrassLoon.Address.Framework
{
    public interface IEmailAddressFactory
    {
        IEmailAddress Create(Guid domainId);
        Task<IEmailAddress> Get(ISettings settings, Guid domainId, Guid id);
    }
}
