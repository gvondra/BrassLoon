using BrassLoon.Interface.Address.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Address
{
    public interface IEmailAddressService
    {
        Task<EmailAddress> Get(ISettings settings, Guid domainId, Guid id);
        Task<EmailAddress> Save(ISettings settings, EmailAddress emailAddress);
    }
}
