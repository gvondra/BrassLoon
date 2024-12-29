using BrassLoon.DataClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.SqlClient
{
    public class PurgeDataSaver : IPurgeDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public PurgeDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public Task DeleteExceptionByMinTimestamp(CommonData.ISettings settings, DateTime timestamp) => DeleteByMinTimestamp(settings, timestamp, "[bll].[DeleteExceptionPurgeByMinTimestamp]");

        public Task DeleteMetricByMinTimestamp(CommonData.ISettings settings, DateTime timestamp) => DeleteByMinTimestamp(settings, timestamp, "[bll].[DeleteMetricPurgeByMinTimestamp]");

        public Task DeleteTraceByMinTimestamp(CommonData.ISettings settings, DateTime timestamp) => DeleteByMinTimestamp(settings, timestamp, "[bll].[DeleteTracePurgeByMinTimestamp]");

        private async Task DeleteByMinTimestamp(CommonData.ISettings settings, DateTime timestamp, string procedureName)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "minTimestamp", DbType.DateTime2, timestamp);
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 150;
                    _ = command.Parameters.Add(parameter);
                    _ = await command.ExecuteNonQueryAsync();
                }
            }
        }

        public Task InitializeException(CommonData.ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp) => Initialize(settings, domainId, expirationTimestamp, maxCreateTimestamp, "[bll].[InitializeExceptionPurge]");

        public Task InitializeMetric(CommonData.ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp) => Initialize(settings, domainId, expirationTimestamp, maxCreateTimestamp, "[bll].[InitializeMetricPurge]");

        public Task InitializeTrace(CommonData.ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp) => Initialize(settings, domainId, expirationTimestamp, maxCreateTimestamp, "[bll].[InitializeTracePurge]");

        private async Task Initialize(CommonData.ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp, string procedureName)
        {
            IDataParameter parameterDomainId = DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId);
            IDataParameter parameterExpirationTimestamp = DataUtil.CreateParameter(_providerFactory, "expirationTimestamp", DbType.DateTime2, expirationTimestamp);
            IDataParameter parameterMaxCcreateTimestamp = DataUtil.CreateParameter(_providerFactory, "maxCreateTimestamp", DbType.DateTime2, maxCreateTimestamp);
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 150;
                    _ = command.Parameters.Add(parameterDomainId);
                    _ = command.Parameters.Add(parameterExpirationTimestamp);
                    _ = command.Parameters.Add(parameterMaxCcreateTimestamp);
                    _ = await command.ExecuteNonQueryAsync();
                }
            }
        }

        public Task PurgeException(CommonData.ISettings settings, Guid domainId, DateTime maxExpirationTimestamp) => Purge(settings, domainId, maxExpirationTimestamp, "[bll].[PurgeException]");

        public Task PurgeMetric(CommonData.ISettings settings, Guid domainId, DateTime maxExpirationTimestamp) => Purge(settings, domainId, maxExpirationTimestamp, "[bll].[PurgeMetric]");

        public Task PurgeTrace(CommonData.ISettings settings, Guid domainId, DateTime maxExpirationTimestamp) => Purge(settings, domainId, maxExpirationTimestamp, "[bll].[PurgeTrace]");

        private async Task Purge(CommonData.ISettings settings, Guid domainId, DateTime maxExpirationTimestamp, string procedureName)
        {
            IDataParameter parameterDomainId = DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId);
            IDataParameter parameterMaxExpirationTimestamp = DataUtil.CreateParameter(_providerFactory, "maxExpirationTimestamp", DbType.DateTime2, maxExpirationTimestamp);
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 150;
                    _ = command.Parameters.Add(parameterDomainId);
                    _ = command.Parameters.Add(parameterMaxExpirationTimestamp);
                    _ = await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
