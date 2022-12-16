using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface ISigningKeyFactory
    {
        ISigningKey Create(Guid domainId);
        Task<ISigningKey> Get(ISettings settings, Guid domainId, Guid id);
        Task<IEnumerable<ISigningKey>> GetByDomainId(ISettings settings, Guid domainId);
    }
}
