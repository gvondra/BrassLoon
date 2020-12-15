using BrassLoon.Interface.Log.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public interface ITraceService
    {
        Task<Trace> Create(ISettings settings, Trace trace);
        Task<Trace> Create(ISettings settings, Guid domainId, string eventCode, string message, object data = null);
        Task<Trace> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, string eventCode, string message, object data = null);
    }
}
