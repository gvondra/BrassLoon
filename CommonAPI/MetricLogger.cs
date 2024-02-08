using BrassLoon.Extensions.Logging;
using Microsoft.Extensions.Logging;

namespace BrassLoon.CommonAPI
{
    public class MetricLogger : IMetricLogger
    {
        public void ApiMethodStats<T>(ILogger<T> logger, Guid domainId, string code, string status, DateTime start, string requestor = "")
            => ApiMethodStats(logger, domainId, code, status, DateTime.UtcNow.Subtract(start.ToUniversalTime()).TotalSeconds, requestor);

        public void ApiMethodStats<T>(ILogger<T> logger, Guid domainId, string code, string status, double? magnitude, string requestor = "")
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));
            _ = logger.LogMetric(new Metric
            {
                EventCode = code,
                DomainId = domainId,
                Magnitude = magnitude,
                Status = !string.IsNullOrEmpty(status) ? status : "Ok",
                Requestor = requestor ?? string.Empty,
                CreateTimestamp = DateTime.UtcNow
            });
        }
    }
}
