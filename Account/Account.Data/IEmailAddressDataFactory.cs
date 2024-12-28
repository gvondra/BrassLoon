using BrassLoon.Account.Data.Models;
using BrassLoon.CommonData;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IEmailAddressDataFactory
    {
        Task<EmailAddressData> Get(ISettings settings, Guid id);
        Task<EmailAddressData> GetByAddress(ISettings settings, string address);
    }
}
