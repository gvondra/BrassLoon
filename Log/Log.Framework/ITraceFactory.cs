using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Framework
{
    public interface ITraceFactory
    {
        ITrace Create(Guid domainId, DateTime? createTimestamp, string eventCode);
    }
}
