using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IUserFactory
    {
        IUser Create(Guid domainId, string referenceId, IEmailAddress emailAddress);
        Task<IUser> Get(ISettings settings, Guid domainId, Guid id);
        Task<IUser> GetByEmailAddress(ISettings settings, Guid domainId, string address);
        Task<IUser> GetByReferenceId(ISettings settings, Guid domainId, string referenceId);
    }
}
