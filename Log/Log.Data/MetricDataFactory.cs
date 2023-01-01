using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public class MetricDataFactory : IMetricDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<MetricData> _genericDataFactory;

        public MetricDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<MetricData>();
        }

        public async Task<IEnumerable<string>> GetEventCodes(ISqlSettings settings, Guid domainId)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId)
            };
            return await DataUtil.ReadList<string>(_providerFactory, settings, "[bll].[GetAllMetricEventCode]", parameters);
        }

        public async Task<IEnumerable<MetricData>> GetTopBeforeTimestamp(ISqlSettings settings, Guid domainId, string eventCode, DateTime maxTimestamp)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(_providerFactory, "eventCode", DbType.AnsiString, eventCode),
                DataUtil.CreateParameter(_providerFactory, "maxTimestamp", DbType.DateTime2, maxTimestamp)
            };

            return await _genericDataFactory.GetData(
                settings, 
                _providerFactory,
                "[bll].[GetTopMetricBeforeTimestamp]", 
                () => new MetricData(),
                DataUtil.AssignDataStateManager,
                parameters
                );
        }
    }
}
