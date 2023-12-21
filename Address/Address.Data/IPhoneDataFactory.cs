using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IPhoneDataFactory
    {
        Task<PhoneData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<PhoneData>> GetByHash(ISqlSettings settings, Guid domainId, byte[] hash);
    }
}
