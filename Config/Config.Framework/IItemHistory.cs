using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Config.Framework
{
    public interface IItemHistory
    {
        Guid ItemHistoryId { get; }
        Guid DomainId { get; }
        string Code { get; }
        dynamic Data { get; }
        DateTime CreateTimestamp { get; }
    }
}
