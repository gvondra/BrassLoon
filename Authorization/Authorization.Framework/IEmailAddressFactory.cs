using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IEmailAddressFactory
    {
        Task<IEmailAddress> Get(ISettings settings, Guid id);
        Task<IEmailAddress> GetByAddress(ISettings settings, string address);
    }
}
