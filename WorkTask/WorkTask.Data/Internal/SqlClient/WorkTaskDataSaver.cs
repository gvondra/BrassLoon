using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class WorkTaskDataSaver : DataSaverBase, IWorkTaskDataSaver
    {
        public WorkTaskDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task<bool> Claim(ISqlTransactionHandler transactionHandler, Guid domainId, Guid id, string userId, DateTime? assignedDate = null)
        {
            await ProviderFactory.EstablishTransaction(transactionHandler);
            using DbCommand command = transactionHandler.Connection.CreateCommand();
            command.CommandText = "[blwt].[ClaimWorkTask]";
            command.CommandType = CommandType.StoredProcedure;
            command.Transaction = transactionHandler.Transaction.InnerTransaction;

            IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
            timestamp.Direction = ParameterDirection.Output;
            _ = command.Parameters.Add(timestamp);

            DataUtil.AddParameter(ProviderFactory, command.Parameters, "workTaskId", DbType.Guid, DataUtil.GetParameterValue(id));
            DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(domainId));
            DataUtil.AddParameter(ProviderFactory, command.Parameters, "userId", DbType.AnsiString, DataUtil.GetParameterValue(userId));
            DataUtil.AddParameter(ProviderFactory, command.Parameters, "assignedDate", DbType.Date, DataUtil.GetParameterValue(assignedDate));

            int count = await command.ExecuteNonQueryAsync();
            return count > 0;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, WorkTaskData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await ProviderFactory.EstablishTransaction(transactionHandler, data);
                using DbCommand command = transactionHandler.Connection.CreateCommand();
                command.CommandText = "[blwt].[CreateWorkTask]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                IDataParameter id = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid);
                id.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(id);

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "workTaskTypeId", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskTypeId));
                AddCommonParameters(command.Parameters, data);

                _ = await command.ExecuteNonQueryAsync();
                data.WorkTaskId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, WorkTaskData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await ProviderFactory.EstablishTransaction(transactionHandler, data);
                using DbCommand command = transactionHandler.Connection.CreateCommand();
                command.CommandText = "[blwt].[UpdateWorkTask]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskId));
                AddCommonParameters(command.Parameters, data);

                _ = await command.ExecuteNonQueryAsync();
                data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }

        private void AddCommonParameters(IList commandParameters, WorkTaskData data)
        {
            DataUtil.AddParameter(ProviderFactory, commandParameters, "workTaskStatusId", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskStatusId));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "title", DbType.String, DataUtil.GetParameterValue(data.Title));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "text", DbType.String, DataUtil.GetParameterValue(data.Text));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "assignedToUserId", DbType.AnsiString, DataUtil.GetParameterValue(data.AssignedToUserId));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "assignedDate", DbType.Date, DataUtil.GetParameterValue(data.AssignedDate));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "closedDate", DbType.Date, DataUtil.GetParameterValue(data.ClosedDate));
        }
    }
}
