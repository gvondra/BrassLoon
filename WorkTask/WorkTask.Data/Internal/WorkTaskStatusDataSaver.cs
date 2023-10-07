using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkTaskStatusDataSaver : DataSaverBase, IWorkTaskStatusDataSaver
    {
        public WorkTaskStatusDataSaver(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public async Task Create(ISqlTransactionHandler transactionHandler, WorkTaskStatusData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[blwt].[CreateWorkTaskStatus]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "code", DbType.AnsiString, DataUtil.GetParameterValue(data.Code));
                    AddCommonParameters(command.Parameters, data);

                    await command.ExecuteNonQueryAsync();
                    data.WorkTaskStatusId = (Guid)id.Value;
                    data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                    data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }

        public async Task Delete(ISqlTransactionHandler transactionHandler, Guid id)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[blwt].[DeleteWorkTaskStatus]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(id));
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, WorkTaskStatusData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[blwt].[UpdateWorkTaskStatus]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskStatusId));
                    AddCommonParameters(command.Parameters, data);

                    await command.ExecuteNonQueryAsync();
                    data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }

        private void AddCommonParameters(IList commandParameters, WorkTaskStatusData data)
        {
            DataUtil.AddParameter(_providerFactory, commandParameters, "workTaskTypeId", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskTypeId));
            DataUtil.AddParameter(_providerFactory, commandParameters, "name", DbType.String, DataUtil.GetParameterValue(data.Name));
            DataUtil.AddParameter(_providerFactory, commandParameters, "description", DbType.String, DataUtil.GetParameterValue(data.Description));
            DataUtil.AddParameter(_providerFactory, commandParameters, "isDefaultStatus", DbType.Boolean, DataUtil.GetParameterValue(data.IsDefaultStatus));
            DataUtil.AddParameter(_providerFactory, commandParameters, "isClosedStatus", DbType.Boolean, DataUtil.GetParameterValue(data.IsClosedStatus));
        }
    }
}
