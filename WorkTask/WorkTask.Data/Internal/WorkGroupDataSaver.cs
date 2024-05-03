using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkGroupDataSaver : DataSaverBase, IWorkGroupDataSaver
    {
        public WorkGroupDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(ISqlTransactionHandler transactionHandler, WorkGroupData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await ProviderFactory.EstablishTransaction(transactionHandler, data);
                using DbCommand command = transactionHandler.Connection.CreateCommand();
                command.CommandText = "[blwt].[CreateWorkGroup]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                IDataParameter id = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid);
                id.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(id);

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                AddCommonParameters(command.Parameters, data);

                _ = await command.ExecuteNonQueryAsync();
                data.WorkGroupId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, WorkGroupData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await ProviderFactory.EstablishTransaction(transactionHandler, data);
                using DbCommand command = transactionHandler.Connection.CreateCommand();
                command.CommandText = "[blwt].[UpdateWorkGroup]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.WorkGroupId));
                AddCommonParameters(command.Parameters, data);

                _ = await command.ExecuteNonQueryAsync();
                data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }

        private void AddCommonParameters(IList commandParameters, WorkGroupData data)
        {
            DataUtil.AddParameter(ProviderFactory, commandParameters, "title", DbType.String, DataUtil.GetParameterValue(data.Title));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "description", DbType.String, DataUtil.GetParameterValue(data.Description));
        }
    }
}
