using BrassLoon.Account.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IDomainDataSaver
    {
        Task Create(ISaveSettings settings, DomainData domainData);
        Task Update(ISaveSettings settings, DomainData domainData);
    }
}
