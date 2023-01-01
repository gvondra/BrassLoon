using BrassLoon.Interface.Log.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public interface IMetricService
    {
        Task<Metric> Create(ISettings settings, Metric metric);
        Task<Metric> Create(ISettings settings, Guid domainId, string eventCode, double magnitude, object data = null);
        Task<Metric> Create(ISettings settings, Guid domainId, string eventCode, double magnitude, string status = "", string requestor = "", object data = null);
        Task<Metric> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, string eventCode, double magnitude, object data = null);
        Task<Metric> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, string eventCode, double magnitude, string status = "", string requestor = "", object data = null);
        Task Create(ISettings settings, Guid domainId, List<Metric> metrics);
        Task<List<string>> GetEventCodes(ISettings settings, Guid domainId);
        Task<List<Metric>> Search(ISettings settings, Guid domainId, DateTime maxTimestamp, string eventCode);
    }
}
