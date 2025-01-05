using BrassLoon.Address.Data.Models;
using BrassLoon.CommonData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IAddressDataFactory
    {
        Task<AddressData> Get(ISettings settings, Guid id);
        Task<IEnumerable<AddressData>> GetByHash(ISettings settings, Guid domainId, byte[] hash);
    }
}
