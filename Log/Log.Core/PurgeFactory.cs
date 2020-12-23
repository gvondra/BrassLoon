using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class PurgeFactory : IPurgeFactory
    {
        private readonly IPurgeDataFactory _dataFactory;
        private readonly IPurgeDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;

        public PurgeFactory(IPurgeDataFactory dataFactory,
            IPurgeDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
        }

        private PurgeMetaData CreateException(PurgeData purgeData)
        {
            return new PurgeMetaData(purgeData,
                _dataSaver,
                (ds, th, d) => ds.CreateException(th, d),
                (ds, th, d) => ds.UpdateException(th, d)
                );
        }

        private PurgeMetaData CreateMetric(PurgeData purgeData)
        {
            return new PurgeMetaData(purgeData,
                _dataSaver,
                (ds, th, d) => ds.CreateMetric(th, d),
                (ds, th, d) => ds.UpdateMetric(th, d)
                );
        }

        private PurgeMetaData CreateTrace(PurgeData purgeData)
        {
            return new PurgeMetaData(purgeData,
                _dataSaver,
                (ds, th, d) => ds.CreateTrace(th, d),
                (ds, th, d) => ds.UpdateTrace(th, d)
                );
        }

        public Task<IPurgeMetaData> GetExceptionByTargetId(ISettings settings, long targetId)
        {
            return GetByTargetId(() => _dataFactory.GetExceptionByTargetId(_settingsFactory.CreateData(settings), targetId), CreateException);
        }

        public Task<IPurgeMetaData> GetMetricByTargetId(ISettings settings, long targetId)
        {
            return GetByTargetId(() => _dataFactory.GetMetricByTargetId(_settingsFactory.CreateData(settings), targetId), CreateMetric);
        }

        public Task<IPurgeMetaData> GetTraceByTargetId(ISettings settings, long targetId)
        {
            return GetByTargetId(() => _dataFactory.GetTraceByTargetId(_settingsFactory.CreateData(settings), targetId), CreateTrace);
        }

        public async Task<IPurgeMetaData> GetByTargetId(Func<Task<PurgeData>> getByTargetId, Func<PurgeData, PurgeMetaData> create)
        {
            IPurgeMetaData result = null;
            PurgeData data = await getByTargetId();
            if (data != null)
                result = create(data);
            return result;
        }

        public IPurgeMetaData CreateException(Guid domainId, long targetId)
        {
            return CreateException(new PurgeData() { DomainId = domainId, TargetId = targetId });
        }

        public IPurgeMetaData CreateMetric(Guid domainId, long targetId)
        {
            return CreateMetric(new PurgeData() { DomainId = domainId, TargetId = targetId });
        }

        public IPurgeMetaData CreateTrace(Guid domainId, long targetId)
        {
            return CreateTrace(new PurgeData() { DomainId = domainId, TargetId = targetId });
        }
    }
}
