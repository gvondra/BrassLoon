using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IUserFactory
    {
        Task<IUser> Get(ISettings settings, Guid id);
        Task<IUser> GetByReferenceId(ISettings settings, string referenceId);
        Task<IEnumerable<IUser>> GetByEmailAddress(ISettings settings, string emailAddress);
        Task<IEnumerable<IUser>> GetByAccountId(ISettings settings, Guid accountId);
        IUser Create(string referenceId, IEmailAddress emailAddress);
    }
}
