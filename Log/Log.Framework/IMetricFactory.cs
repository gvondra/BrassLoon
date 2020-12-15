using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Framework
{
    public interface IMetricFactory
    {
        IMetric Create(Guid domainId, DateTime? createTimestamp, string eventCode);
    }
}
