using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IAddressDataFactory
    {
        Task<AddressData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<AddressData>> GetByHash(ISqlSettings settings, byte[] hash);
    }
}
