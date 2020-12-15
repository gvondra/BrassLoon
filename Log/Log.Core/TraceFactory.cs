using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Core
{
    public class TraceFactory : ITraceFactory
    {
        private readonly ITraceDataSaver _dataSaver;

        public TraceFactory(ITraceDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        public ITrace Create(Guid domainId, DateTime? createTimestamp, string eventCode)
        {
            if (!createTimestamp.HasValue)
                createTimestamp = DateTime.UtcNow;
            createTimestamp = createTimestamp.Value.ToUniversalTime();
            return new Trace(
                new TraceData()
                {
                    DomainId = domainId,
                    EventCode = (eventCode ?? string.Empty).Trim(),
                    CreateTimestamp = createTimestamp.Value
                },
                _dataSaver
                );
        }
    }
}
