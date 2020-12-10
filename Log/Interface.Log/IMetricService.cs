using BrassLoon.Interface.Log.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public interface IMetricService
    {
        Task<Metric> Create(ISettings settings, Metric metric);
        Task<Metric> Create(ISettings settings, Guid domainId, string eventCode, double maginitue, object data = null);
    }
}
