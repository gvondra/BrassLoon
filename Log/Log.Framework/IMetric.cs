using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IMetric
    {
        long? LegacyMetricId { get; }
        Guid MetricId { get; }
        Guid DomainId { get; }
        string EventCode { get; }
        double? Magnitude { get; set; }
        dynamic Data { get; set; }
        DateTime CreateTimestamp { get; }
        string Status { get; set; }
        string Requestor { get; set; }
        string Category { get; set; }
        string Level { get; set; }

        Task Create(ISaveSettings settings);
    }
}
