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

        public Task CreateException(ISqlTransactionHandler transactionHandler, PurgeData purgeData)
        {
            return Create(transactionHandler, purgeData, "[bll].[CreateExceptionPurge]");
        }

        public Task CreateMetric(ISqlTransactionHandler transactionHandler, PurgeData purgeData)
        {
            return Create(transactionHandler, purgeData, "[bll].[CreateMetricPurge]");
        }

        public Task CreateTrace(ISqlTransactionHandler transactionHandler, PurgeData purgeData)
        {
            return Create(transactionHandler, purgeData, "[bll].[CreateTracePurge]");
        }

        private async Task Create(ISqlTransactionHandler transactionHandler, PurgeData purgeData, string procedureName)
        {
            if (purgeData.Manager.GetState(purgeData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, purgeData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Int64);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(purgeData.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "status", DbType.Int16, DataUtil.GetParameterValue(purgeData.Status));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "targetId", DbType.Int64, DataUtil.GetParameterValue(purgeData.TargetId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "expirationTimestamp", DbType.DateTime2, DataUtil.GetParameterValue(purgeData.ExpirationTimestamp));

                    await command.ExecuteNonQueryAsync();
                    purgeData.PurgeId = (long)id.Value;
                    purgeData.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                    purgeData.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
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

        public Task UpdateException(ISqlTransactionHandler transactionHandler, PurgeData purgeData)
        {
            return Update(transactionHandler, purgeData, "[bll].[UpdateExceptionPurge]");
        }

        public Task UpdateMetric(ISqlTransactionHandler transactionHandler, PurgeData purgeData)
        {
            return Update(transactionHandler, purgeData, "[bll].[UpdateMetricPurge]");
        }

        public Task UpdateTrace(ISqlTransactionHandler transactionHandler, PurgeData purgeData)
        {
            return Update(transactionHandler, purgeData, "[bll].[UpdateTracePurge]");
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, PurgeData purgeData, string procedureName)
        {
            if (purgeData.Manager.GetState(purgeData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, purgeData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Int64, DataUtil.GetParameterValue(purgeData.PurgeId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "status", DbType.Int16, DataUtil.GetParameterValue(purgeData.Status));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "expirationTimestamp", DbType.DateTime2, DataUtil.GetParameterValue(purgeData.ExpirationTimestamp));

                    await command.ExecuteNonQueryAsync();
                    purgeData.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
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
    }
}
