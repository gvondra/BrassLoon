using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class WorkTaskContextDataSaver : DataSaverBase, IWorkTaskContextDataSaver
    {
        public WorkTaskContextDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(ISqlTransactionHandler transactionHandler, WorkTaskContextData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await ProviderFactory.EstablishTransaction(transactionHandler, data);
                using DbCommand command = transactionHandler.Connection.CreateCommand();
                command.CommandText = "[blwt].[CreateWorkTaskContext]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                IDataParameter id = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid);
                id.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(id);

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "workTaskId", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "status", DbType.Int16, DataUtil.GetParameterValue(data.Status));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "referenceType", DbType.Int16, DataUtil.GetParameterValue(data.ReferenceType));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "referenceValue", DbType.String, DataUtil.GetParameterValue(data.ReferenceValue));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "referenceValueHash", DbType.Binary, DataUtil.GetParameterValue(data.ReferenceValueHash));

                _ = await command.ExecuteNonQueryAsync();
                data.WorkTaskId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }
    }
}
