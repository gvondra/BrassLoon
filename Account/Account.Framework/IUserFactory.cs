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
        IUser Create(string referenceId, IEmailAddress emailAddress);
    }
}
