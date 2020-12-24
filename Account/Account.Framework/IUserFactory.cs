using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IUserFactory
    {
        Task<IUser> Get(ISettings settings, Guid id);
        Task<IUser> GetByReferenceId(ISettings settings, string referenceId);
        Task<IEnumerable<IUser>> GetByEmailAddress(ISettings settings, string emailAddress);
        IUser Create(string referenceId, IEmailAddress emailAddress);
    }
}
