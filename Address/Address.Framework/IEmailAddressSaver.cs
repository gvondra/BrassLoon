using System.Threading.Tasks;

namespace BrassLoon.Address.Framework
{
    public interface IEmailAddressSaver
    {
        Task<IEmailAddress> Save(ISettings settings, IEmailAddress emailAddress);
    }
}
