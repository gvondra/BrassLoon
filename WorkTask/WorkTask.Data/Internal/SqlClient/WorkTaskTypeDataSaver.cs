using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public class WorkTaskTypeDataSaver : DataSaverBase, IWorkTaskTypeDataSaver
    {
        public WorkTaskTypeDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(CommonData.ISaveSettings settings, WorkTaskTypeData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await ProviderFactory.EstablishTransaction(settings, data);
                using DbCommand command = settings.Connection.CreateCommand();
                command.CommandText = "[blwt].[CreateWorkTaskType]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                IDataParameter id = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid);
                id.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(id);

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "code", DbType.String, DataUtil.GetParameterValue(data.Code));
                AddCommonParameters(command.Parameters, data);

                _ = await command.ExecuteNonQueryAsync();
                data.WorkTaskTypeId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }

        public async Task Update(CommonData.ISaveSettings settings, WorkTaskTypeData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await ProviderFactory.EstablishTransaction(settings, data);
                using DbCommand command = settings.Connection.CreateCommand();
                command.CommandText = "[blwt].[UpdateWorkTaskType]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.WorkTaskTypeId));
                AddCommonParameters(command.Parameters, data);

                _ = await command.ExecuteNonQueryAsync();
                data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }

        private void AddCommonParameters(IList commandParameters, WorkTaskTypeData data)
        {
            DataUtil.AddParameter(ProviderFactory, commandParameters, "title", DbType.String, DataUtil.GetParameterValue(data.Title));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "description", DbType.String, DataUtil.GetParameterValue(data.Description));
            DataUtil.AddParameter(ProviderFactory, commandParameters, "purgePeriod", DbType.Int16, DataUtil.GetParameterValue(data.PurgePeriod));
        }
    }
}
