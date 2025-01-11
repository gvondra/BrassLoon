using BrassLoon.Authorization.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IUserDataFactory
    {
        Task<UserData> Get(CommonData.ISettings settings, Guid id);
        Task<UserData> GetByEmailAddressHash(CommonData.ISettings settings, Guid domainId, byte[] hash);
        Task<UserData> GetByReferenceId(CommonData.ISettings settings, Guid domainId, string referenceId);
        Task<IEnumerable<UserData>> GetByDomainId(CommonData.ISettings settings, Guid domainId);
    }
}
