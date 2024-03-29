﻿using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface ITraceFactory
    {
        ITrace Create(Guid domainId, DateTime? createTimestamp, string eventCode, IEventId eventId = null);
        Task<IEnumerable<string>> GetEventCodes(ISettings settings, Guid domainId);
        Task<IEnumerable<ITrace>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, string eventCode, DateTime maxTimestamp);
    }
}
