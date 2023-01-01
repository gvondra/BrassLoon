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
    public class MetricFactory : IMetricFactory
    {
        private readonly IMetricDataFactory _dataFactory;
        private readonly IMetricDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;

        public MetricFactory(IMetricDataFactory dataFactory,
            IMetricDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
        }

        private Metric Create(MetricData data) => new Metric(data, _dataSaver);
        private Metric Create(MetricData data, IEventId eventId) => new Metric(data, _dataSaver, eventId);

        public IMetric Create(Guid domainId, DateTime? createTimestamp, string eventCode, IEventId eventId = null)
        {
            if (!createTimestamp.HasValue)
                createTimestamp = DateTime.UtcNow;
            createTimestamp = createTimestamp.Value.ToUniversalTime();
            return Create(
                new MetricData()
                { 
                    DomainId = domainId,
                    EventCode = eventCode,
                    CreateTimestamp = createTimestamp.Value
                },
                eventId
                );
        }

        public async Task<IEnumerable<string>> GetEventCodes(ISettings settings, Guid domainId)
        {
            return await _dataFactory.GetEventCodes(_settingsFactory.CreateData(settings), domainId);
        }

        public async Task<IEnumerable<IMetric>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, string eventCode, DateTime maxTimestamp)
        {
            return (await _dataFactory.GetTopBeforeTimestamp(_settingsFactory.CreateData(settings), domainId, eventCode, maxTimestamp.ToUniversalTime()))
                .Select<MetricData, IMetric>(Create)
                ;
        }
    }
}
