using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IMetric
    {
        long MetricId { get; }
        Guid DomainId { get; }
        string EventCode { get; }
        double? Magnitude { get; set; }
        dynamic Data { get; set; }
        DateTime CreateTimestamp { get; }
        string Status { get; set; }
        string Requestor { get; set; }
        string Category { get; set; }
        string Level { get; set; }

        Task Create(ITransactionHandler transactionHandler);
    }
}
