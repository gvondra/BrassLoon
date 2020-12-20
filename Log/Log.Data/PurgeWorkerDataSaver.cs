using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public class PurgeWorkerDataSaver : IPurgeWorkerDataSaver
    {
        private ISqlDbProviderFactory _providerFactory;

        public PurgeWorkerDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task InitializePurgeWorker(ISqlSettings settings)
        {
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[bll].[InitializePurgeWorker]";
                    command.CommandType = CommandType.StoredProcedure;
                    await command.ExecuteNonQueryAsync();
                }
            }            
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, PurgeWorkerData purgeWorkerData)
        {
            if (purgeWorkerData.Manager.GetState(purgeWorkerData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, purgeWorkerData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bll].[UpdatePurgeWorker]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "purgeWorkerId", DbType.Guid, DataUtil.GetParameterValue(purgeWorkerData.PurgeWorkerId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "status", DbType.Int16, DataUtil.GetParameterValue(purgeWorkerData.Status));

                    await command.ExecuteNonQueryAsync();
                    purgeWorkerData.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }
    }
}
