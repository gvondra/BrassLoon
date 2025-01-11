using BrassLoon.Authorization.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IEmailAddressDataFactory
    {
        Task<EmailAddressData> Get(CommonData.ISettings settings, Guid id);
        Task<EmailAddressData> GetByAddressHash(CommonData.ISettings settings, byte[] hash);
    }
}
