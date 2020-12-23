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
    public class PurgeDataSaver : IPurgeDataSaver
    {
        private ISqlDbProviderFactory _providerFactory;

        public PurgeDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public Task DeleteExceptionByMinTimestamp(ISqlSettings settings, DateTime timestamp)
        {
            return DeleteByMinTimestamp(settings, timestamp, "[bll].[DeleteExceptionPurgeByMinTimestamp]");
        }

        public Task DeleteMetricByMinTimestamp(ISqlSettings settings, DateTime timestamp)
        {
            return DeleteByMinTimestamp(settings, timestamp, "[bll].[DeleteMetricPurgeByMinTimestamp]");
        }

        public Task DeleteTraceByMinTimestamp(ISqlSettings settings, DateTime timestamp)
        {
            return DeleteByMinTimestamp(settings, timestamp, "[bll].[DeleteTracePurgeByMinTimestamp]");
        }

        private async Task DeleteByMinTimestamp(ISqlSettings settings, DateTime timestamp, string procedureName)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "minTimestamp", DbType.DateTime2, timestamp);
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(parameter);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public Task InitializeException(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp)
        {
            return Initialize(settings, domainId, expirationTimestamp, maxCreateTimestamp, "[bll].[InitializeExceptionPurge]");
        }

        public Task InitializeMetric(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp)
        {
            return Initialize(settings, domainId, expirationTimestamp, maxCreateTimestamp, "[bll].[InitializeMetricPurge]");
        }

        public Task InitializeTrace(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp)
        {
            return Initialize(settings, domainId, expirationTimestamp, maxCreateTimestamp, "[bll].[InitializeTracePurge]");
        }

        private async Task Initialize(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp, string procedureName)
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
                    command.Parameters.Add(parameterDomainId);
                    command.Parameters.Add(parameterExpirationTimestamp);
                    command.Parameters.Add(parameterMaxCcreateTimestamp);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public Task PurgeException(ISqlSettings settings, Guid domainId, DateTime maxExpirationTimestamp)
        {
            return Purge(settings, domainId, maxExpirationTimestamp, "[bll].[PurgeException]");
        }

        public Task PurgeMetric(ISqlSettings settings, Guid domainId, DateTime maxExpirationTimestamp)
        {
            return Purge(settings, domainId, maxExpirationTimestamp, "[bll].[PurgeMetric]");
        }

        public Task PurgeTrace(ISqlSettings settings, Guid domainId, DateTime maxExpirationTimestamp)
        {
            return Purge(settings, domainId, maxExpirationTimestamp, "[bll].[PurgeTrace]");
        }

        private async Task Purge(ISqlSettings settings, Guid domainId, DateTime maxExpirationTimestamp, string procedureName)
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
                    command.Parameters.Add(parameterDomainId);
                    command.Parameters.Add(parameterMaxExpirationTimestamp);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
