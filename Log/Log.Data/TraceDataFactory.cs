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
    public class TraceDataFactory : ITraceDataFactory
    {
        private readonly IDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<TraceData> _genericDataFactory;

        public TraceDataFactory(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<TraceData>();
        }

        public async Task<IEnumerable<string>> GetEventCodes(ISettings settings, Guid domainId)
        {
            List<string> result = new List<string>();
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[bll].[GetAllTraceEventCode]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId));
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        result.Add(await reader.GetFieldValueAsync<string>(0));
                    }
                }
                connection.Close();
            }
            return result;
        }

        public async Task<IEnumerable<TraceData>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, string eventCode, DateTime maxTimestamp)
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
                parameters
                );
        }
    }
}
