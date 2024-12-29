using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.SqlClient
{
    public class PurgeWorkerDataSaver : IPurgeWorkerDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public PurgeWorkerDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task InitializePurgeWorker(CommonData.ISettings settings)
        {
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[bll].[InitializePurgeWorker]";
                    command.CommandType = CommandType.StoredProcedure;
                    _ = await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task Update(CommonData.ISaveSettings settings, PurgeWorkerData purgeWorkerData)
        {
            if (purgeWorkerData.Manager.GetState(purgeWorkerData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(settings, purgeWorkerData);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[bll].[UpdatePurgeWorker]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "purgeWorkerId", DbType.Guid, DataUtil.GetParameterValue(purgeWorkerData.PurgeWorkerId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "status", DbType.Int16, DataUtil.GetParameterValue(purgeWorkerData.Status));

                    _ = await command.ExecuteNonQueryAsync();
                    purgeWorkerData.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }
    }
}
