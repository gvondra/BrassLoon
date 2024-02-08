using Microsoft.Extensions.Logging;

namespace BrassLoon.CommonAPI
{
    public interface IMetricLogger
    {
        void ApiMethodStats<T>(ILogger<T> logger, Guid domainId, string code, string status, DateTime start, string requestor = "");
        void ApiMethodStats<T>(ILogger<T> logger, Guid domainId, string code, string status, double? magnitude, string requestor = "");
    }
}
