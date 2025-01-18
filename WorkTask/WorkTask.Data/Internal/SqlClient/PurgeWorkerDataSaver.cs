using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class PurgeWorkerDataSaver : DataSaverBase, IPurgeWorkerDataSaver
    {
        public PurgeWorkerDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task InitializePurgeWorker(CommonData.ISettings settings)
        {
            using DbConnection connection = await ProviderFactory.OpenConnection(settings);
            using DbCommand command = connection.CreateCommand();
            command.CommandText = "[blwt].[InitializePurgeWorker]";
            command.CommandType = CommandType.StoredProcedure;
            _ = await command.ExecuteNonQueryAsync();
        }
        public async Task Update(CommonData.ISaveSettings settings, PurgeWorkerData purgeWorkerData)
        {
            if (purgeWorkerData.Manager.GetState(purgeWorkerData) == DataState.Updated)
            {
                await ProviderFactory.EstablishTransaction(settings, purgeWorkerData);
                using DbCommand command = settings.Connection.CreateCommand();
                command.CommandText = "[blwt].[UpdatePurgeWorker]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "purgeWorkerId", DbType.Guid, DataUtil.GetParameterValue(purgeWorkerData.PurgeWorkerId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "status", DbType.Int16, DataUtil.GetParameterValue(purgeWorkerData.Status));

                _ = await command.ExecuteNonQueryAsync();
                purgeWorkerData.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }
    }
}
