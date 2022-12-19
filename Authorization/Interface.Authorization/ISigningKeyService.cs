using BrassLoon.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public interface ISigningKeyService
    {
        Task<List<SigningKey>> GetByDomain(ISettings settings, Guid domainId);
        Task<SigningKey> Create(ISettings settings, Guid domainId, SigningKey signingKey);
        Task<SigningKey> Create(ISettings settings, SigningKey signingKey);
        Task<SigningKey> Update(ISettings settings, Guid domainId, Guid signingKeyId, SigningKey signingKey);
        Task<SigningKey> Update(ISettings settings, SigningKey signingKey);
    }
}
