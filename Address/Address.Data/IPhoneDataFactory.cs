using BrassLoon.Address.Data.Models;
using BrassLoon.CommonData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IPhoneDataFactory
    {
        Task<PhoneData> Get(ISettings settings, Guid id);
        Task<IEnumerable<PhoneData>> GetByHash(ISettings settings, Guid domainId, byte[] hash);
    }
}
