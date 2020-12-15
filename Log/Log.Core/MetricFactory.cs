using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Core
{
    public class MetricFactory : IMetricFactory
    {
        private readonly IMetricDataSaver _dataSaver;

        public MetricFactory(IMetricDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        public IMetric Create(Guid domainId, DateTime? createTimestamp, string eventCode)
        {
            if (!createTimestamp.HasValue)
                createTimestamp = DateTime.UtcNow;
            createTimestamp = createTimestamp.Value.ToUniversalTime();
            return new Metric(
                new MetricData()
                { 
                    DomainId = domainId,
                    EventCode = eventCode,
                    CreateTimestamp = createTimestamp.Value
                },
                _dataSaver
                );
        }
    }
}
