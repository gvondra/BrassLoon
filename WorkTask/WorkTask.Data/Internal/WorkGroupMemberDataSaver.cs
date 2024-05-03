using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkGroupMemberDataSaver : DataSaverBase, IWorkGroupMemberDataSaver
    {
        public WorkGroupMemberDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(ISqlTransactionHandler transactionHandler, WorkGroupMemberData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await ProviderFactory.EstablishTransaction(transactionHandler, data);
                using DbCommand command = transactionHandler.Connection.CreateCommand();
                command.CommandText = "[blwt].[CreateWorkGroupMember]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                IDataParameter id = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid);
                id.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(id);

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "workGroupId", DbType.Guid, DataUtil.GetParameterValue(data.WorkGroupId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "userId", DbType.AnsiString, DataUtil.GetParameterValue(data.UserId));

                _ = await command.ExecuteNonQueryAsync();
                data.WorkGroupMemberId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }

        public async Task Delete(ISqlTransactionHandler transactionHandler, Guid id)
        {
            await ProviderFactory.EstablishTransaction(transactionHandler);
            using DbCommand command = transactionHandler.Connection.CreateCommand();
            command.CommandText = "[blwt].[DeleteWorkGroupMember]";
            command.CommandType = CommandType.StoredProcedure;
            command.Transaction = transactionHandler.Transaction.InnerTransaction;

            DataUtil.AddParameter(ProviderFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(id));

            _ = await command.ExecuteNonQueryAsync();
        }
    }
}
