using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IUserDataFactory
    {
        Task<UserData> Get(ISettings settings, Guid id);
        Task<UserData> GetByReferenceId(ISettings settings, string referenceId);
    }
}
