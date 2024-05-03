using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class WorkTaskCommentDataSaver : DataSaverBase, IWorkTaskCommentDataSaver
    {
        public WorkTaskCommentDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(ISqlTransactionHandler transactionHandler, CommentData data, Guid workTaskId)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await ProviderFactory.EstablishTransaction(transactionHandler, data);
                using DbCommand command = transactionHandler.Connection.CreateCommand();
                command.CommandText = "[blwt].[CreateWorkTaskComment]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                IDataParameter id = DataUtil.CreateParameter(ProviderFactory, "commentId", DbType.Guid);
                id.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(id);

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "workTaskId", DbType.Guid, DataUtil.GetParameterValue(workTaskId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "text", DbType.String, DataUtil.GetParameterValue(data.Text));

                _ = await command.ExecuteNonQueryAsync();
                data.CommentId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }
    }
}
