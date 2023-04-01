using BrassLoon.DataClient;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkTaskTypeGroupDataSaver : DataSaverBase, IWorkTaskTypeGroupDataSaver
    {
        public WorkTaskTypeGroupDataSaver(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public async Task Create(ISqlTransactionHandler transactionHandler, Guid workTaskTypeId, Guid workGroupId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[blwt].[CreateWorktTaskTypeGroup]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "workTaskTypeId", DbType.Guid, DataUtil.GetParameterValue(workTaskTypeId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "workGroupId", DbType.Guid, DataUtil.GetParameterValue(workGroupId));

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Delete(ISqlTransactionHandler transactionHandler, Guid workTaskTypeId, Guid workGroupId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[blwt].[DeleteWorktTaskTypeGroup]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "workTaskTypeId", DbType.Guid, DataUtil.GetParameterValue(workTaskTypeId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "workGroupId", DbType.Guid, DataUtil.GetParameterValue(workGroupId));

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
