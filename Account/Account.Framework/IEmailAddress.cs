using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IEmailAddress
    {
        Guid EmailAddressId { get; }
        string Address { get; }
        DateTime CreateTimestamp { get; }

        Task Create(CommonCore.ISaveSettings saveSettings);
    }
}
