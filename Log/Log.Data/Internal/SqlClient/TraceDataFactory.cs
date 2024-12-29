using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.SqlClient
{
    public class TraceDataFactory : ITraceDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<TraceData> _genericDataFactory;

        public TraceDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<TraceData>();
        }

        public async Task<IEnumerable<string>> GetEventCodes(ISqlSettings settings, Guid domainId)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId)
            };
            return await DataUtil.ReadList<string>(_providerFactory, settings, "[bll].[GetAllTraceEventCode]", parameters);
        }

        public async Task<IEnumerable<TraceData>> GetTopBeforeTimestamp(ISqlSettings settings, Guid domainId, string eventCode, DateTime maxTimestamp)
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
                "[bll].[GetTopTraceBeforeTimestamp]",
                () => new TraceData(),
                DataUtil.AssignDataStateManager,
                parameters);
        }
    }
}
