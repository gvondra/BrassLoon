using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IEmailAddressDataFactory
    {
        Task<EmailAddressData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<EmailAddressData>> GetByHash(ISqlSettings settings, Guid domainId, byte[] hash);
    }
}
