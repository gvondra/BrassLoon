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

        private Trace Create(TraceData data) => new Trace(data, _dataSaver);
        private Trace Create(TraceData data, IEventId eventId) => new Trace(data, _dataSaver, eventId);

        public ITrace Create(Guid domainId, DateTime? createTimestamp, string eventCode, IEventId eventId = null)
        {
            if (!createTimestamp.HasValue)
                createTimestamp = DateTime.UtcNow;
            createTimestamp = createTimestamp.Value.ToUniversalTime();
            return Create(
                new TraceData()
                {
                    DomainId = domainId,
                    EventCode = (eventCode ?? string.Empty).Trim(),
                    CreateTimestamp = createTimestamp.Value
                },
                eventId
                );
        }

        public Task<IEnumerable<string>> GetEventCodes(ISettings settings, Guid domainId) => _dataFactory.GetEventCodes(_settingFactory.CreateData(settings), domainId);

        public async Task<IEnumerable<ITrace>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, string eventCode, DateTime maxTimestamp)
        {
            return (await _dataFactory.GetTopBeforeTimestamp(_settingFactory.CreateData(settings), domainId, eventCode, maxTimestamp.ToUniversalTime()))
                .Select<TraceData, ITrace>(Create);
        }
    }
}
