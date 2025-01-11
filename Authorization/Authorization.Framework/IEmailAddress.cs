using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IEmailAddress
    {
        Guid EmailAddressId { get; }
        string Address { get; }
        byte[] AddressHash { get; }
        DateTime CreateTimestamp { get; }
        bool IsNew { get; }

        Task Create(CommonCore.ISaveSettings settings);
    }
}
