using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public interface IDomainService
    {
        Task<Domain> Get(ISettings settings, Guid id);
    }
}
