using BrassLoon.Account.Data.Models;
using BrassLoon.CommonData;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IDomainDataSaver
    {
        Task Create(ISaveSettings settings, DomainData domainData);
        Task Update(ISaveSettings settings, DomainData domainData);
    }
}
