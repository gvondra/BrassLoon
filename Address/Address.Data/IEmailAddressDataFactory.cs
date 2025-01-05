using BrassLoon.Address.Data.Models;
using BrassLoon.CommonData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IEmailAddressDataFactory
    {
        Task<EmailAddressData> Get(ISettings settings, Guid id);
        Task<IEnumerable<EmailAddressData>> GetByHash(ISettings settings, Guid domainId, byte[] hash);
    }
}
