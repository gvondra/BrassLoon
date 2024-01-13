using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IEmailAddressSaver
    {
        Task Create(ISettings settings, IEmailAddress emailAddress);
    }
}
