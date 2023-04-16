using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkTaskDataSaver : DataSaverBase, IWorkTaskDataSaver
    {
        public WorkTaskDataSaver(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public async Task<bool> Claim(ISqlTransactionHandler transactionHandler, Guid domainId, Guid id, string userId, DateTime? assignedDate = null)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[blwt].[ClaimWorkTask]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                command.Parameters.Add(timestamp);

                DataUtil.AddParameter(_providerFactory, command.Parameters, "workTaskId", DbType.Guid, DataUtil.GetParameterValue(id));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(domainId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.AnsiString, DataUtil.GetParameterValue(userId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "assignedDate", DbType.Date, DataUtil.GetParameterValue(assignedDate));

                int count = await command.ExecuteNonQueryAsync();
                return count > 0;
            }
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, WorkTaskData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[blwt].[CreateWorkTask]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "workTaskTypeId", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskTypeId));
                    AddCommonParameters(command.Parameters, data);

                    await command.ExecuteNonQueryAsync();
                    data.WorkTaskId = (Guid)id.Value;
                    data.CreateTimestamp = (DateTime)timestamp.Value;
                    data.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, WorkTaskData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[blwt].[UpdateWorkTask]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskId));
                    AddCommonParameters(command.Parameters, data);

                    await command.ExecuteNonQueryAsync();
                    data.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        private void AddCommonParameters(IList commandParameters, WorkTaskData data)
        {
            DataUtil.AddParameter(_providerFactory, commandParameters, "workTaskStatusId", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskStatusId));
            DataUtil.AddParameter(_providerFactory, commandParameters, "title", DbType.String, DataUtil.GetParameterValue(data.Title));
            DataUtil.AddParameter(_providerFactory, commandParameters, "text", DbType.String, DataUtil.GetParameterValue(data.Text));
            DataUtil.AddParameter(_providerFactory, commandParameters, "assignedToUserId", DbType.AnsiString, DataUtil.GetParameterValue(data.AssignedToUserId));
            DataUtil.AddParameter(_providerFactory, commandParameters, "assignedDate", DbType.Date, DataUtil.GetParameterValue(data.AssignedDate));
        }
    }
}
