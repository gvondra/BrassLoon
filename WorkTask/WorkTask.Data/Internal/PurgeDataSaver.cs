using BrassLoon.DataClient;
using System.Data.Common;
using System.Data;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class PurgeDataSaver : DataSaverBase, IPurgeDataSaver
    {
        public PurgeDataSaver(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public Task DeleteWorkTaskByMinTimestamp(ISqlSettings settings, DateTime timestamp)
            => DeleteByMinTimestamp(settings, timestamp, "[blwt].[DeleteWorkTaskPurge_by_MinTimestamp]");

        private async Task DeleteByMinTimestamp(ISqlSettings settings, DateTime timestamp, string procedureName)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "minTimestamp", DbType.DateTime2, timestamp);
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 150;
                    command.Parameters.Add(parameter);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public Task InitializeWorkTask(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, short defaultPurgePeriod)
            => Initialize(settings, domainId, expirationTimestamp, defaultPurgePeriod, "[blwt].[InitializeWorkTaskPurge]");

        private async Task Initialize(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, short defaultPurgePeriod, string procedureName)
        {
            IDataParameter parameterDomainId = DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId);
            IDataParameter parameterExpirationTimestamp = DataUtil.CreateParameter(_providerFactory, "expirationTimestamp", DbType.DateTime2, expirationTimestamp);
            IDataParameter parameterMaxCcreateTimestamp = DataUtil.CreateParameter(_providerFactory, "defaultPurgePeriod", DbType.Int16, defaultPurgePeriod);
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
        public Task PurgeWorkTask(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp)
            => Purge(settings, domainId, expirationTimestamp, "[blwt].[PurgeWorkTask]");

        private async Task Purge(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, string procedureName)
        {
            IDataParameter parameterDomainId = DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId);
            IDataParameter parameterMaxExpirationTimestamp = DataUtil.CreateParameter(_providerFactory, "expirationTimestamp", DbType.DateTime2, expirationTimestamp);
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
