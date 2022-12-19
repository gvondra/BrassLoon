using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Framework
{
    public interface IEmailAddressDataFactory
    {
        Task<EmailAddressData> Get(ISqlSettings settings, Guid id);
        Task<EmailAddressData> GetByAddressHash(ISqlSettings settings, byte[] hash);
    }
}
