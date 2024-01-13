using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IDomainSaver
    {
        Task Create(ISettings settings, params IDomain[] domains);
        Task Update(ISettings settings, params IDomain[] domains);
    }
}
