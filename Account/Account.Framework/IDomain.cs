using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Framework
{
    public interface IDomain
    {
        Guid DomainId { get; }
        string Name { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }
    }
}
