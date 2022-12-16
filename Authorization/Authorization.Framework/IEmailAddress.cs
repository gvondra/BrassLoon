using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IEmailAddress
    {
        Guid EmailAddressId { get; }
        string Address { get; }
        byte[] AddressHash { get; }
        DateTime CreateTimestamp { get; }

        Task Create(ITransactionHandler transactionHandler);
    }
}
