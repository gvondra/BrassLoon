using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Config.Framework
{
    public interface ILookupHistory
    {
        Guid LookupHistoryId { get; }
        Guid DomainId { get; }
        string Code { get; }
        Dictionary<string, string> Data { get; }
        DateTime CreateTimestamp { get; }
    }
}
