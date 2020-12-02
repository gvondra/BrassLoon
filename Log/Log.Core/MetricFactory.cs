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

        public IMetric Create(Guid domainId, string eventCode)
        {
            return new Metric(
                new MetricData()
                { 
                    DomainId = domainId,
                    EventCode = eventCode
                },
                _dataSaver
                );
        }
    }
}
