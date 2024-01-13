using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IEmailAddressFactory
    {
        Task<IEmailAddress> Get(ISettings settings, Guid id);
        Task<IEmailAddress> GetByAddress(ISettings settings, string address);
        IEmailAddress Create(string address);
    }
}
