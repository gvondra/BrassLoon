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
        private readonly IDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<MetricData> _genericDataFactory;

        public MetricDataFactory(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<MetricData>();
        }

        public async Task<IEnumerable<string>> GetEventCodes(ISettings settings, Guid domainId)
        {
            List<string> result = new List<string>();
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[bll].[GetAllMetricEventCode]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId));
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(await reader.GetFieldValueAsync<string>(0));
                        }                        
                    }
                }
                connection.Close();
            }
            return result;
        }

        public async Task<IEnumerable<MetricData>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, string eventCode, DateTime maxTimestamp)
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
