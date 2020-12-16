using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class TraceFactory : ITraceFactory
    {
        private readonly ITraceDataFactory _dataFactory;
        private readonly ITraceDataSaver _dataSaver;
        private readonly SettingsFactory _settingFactory;

        public TraceFactory(ITraceDataFactory dataFactory,
            ITraceDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingFactory = settingsFactory;
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

        public Task<IEnumerable<string>> GetEventCodes(ISettings settings, Guid domainId)
        {
            return _dataFactory.GetEventCodes(_settingFactory.CreateData(settings), domainId);
        }

        public async Task<IEnumerable<ITrace>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, string eventCode, DateTime maxTimestamp)
        {
            return (await _dataFactory.GetTopBeforeTimestamp(_settingFactory.CreateData(settings), domainId, eventCode, maxTimestamp))
                .Select<TraceData, ITrace>(data => new Trace(data, _dataSaver));
                ;
        }
    }
}
