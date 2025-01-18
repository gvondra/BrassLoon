using BrassLoon.DataClient;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class WorkTaskTypeGroupDataSaver : DataSaverBase, IWorkTaskTypeGroupDataSaver
    {
        public WorkTaskTypeGroupDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(CommonData.ISaveSettings settings, Guid domainId, Guid workTaskTypeId, Guid workGroupId)
        {
            await ProviderFactory.EstablishTransaction(settings);
            using DbCommand command = settings.Connection.CreateCommand();
            command.CommandText = "[blwt].[CreateWorktTaskTypeGroup_v2]";
            command.CommandType = CommandType.StoredProcedure;
            command.Transaction = settings.Transaction.InnerTransaction;

            DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(domainId));
            DataUtil.AddParameter(ProviderFactory, command.Parameters, "workTaskTypeId", DbType.Guid, DataUtil.GetParameterValue(workTaskTypeId));
            DataUtil.AddParameter(ProviderFactory, command.Parameters, "workGroupId", DbType.Guid, DataUtil.GetParameterValue(workGroupId));

            _ = await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(CommonData.ISaveSettings settings, Guid domainId, Guid workTaskTypeId, Guid workGroupId)
        {
            await ProviderFactory.EstablishTransaction(settings);
            using DbCommand command = settings.Connection.CreateCommand();
            command.CommandText = "[blwt].[DeleteWorktTaskTypeGroup_v2]";
            command.CommandType = CommandType.StoredProcedure;
            command.Transaction = settings.Transaction.InnerTransaction;

            DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(domainId));
            DataUtil.AddParameter(ProviderFactory, command.Parameters, "workTaskTypeId", DbType.Guid, DataUtil.GetParameterValue(workTaskTypeId));
            DataUtil.AddParameter(ProviderFactory, command.Parameters, "workGroupId", DbType.Guid, DataUtil.GetParameterValue(workGroupId));

            _ = await command.ExecuteNonQueryAsync();
        }
    }
}
