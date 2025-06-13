using BrassLoon.DataClient;
using BrassLoon.DataClient.SqlClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.SqlClient
{
    public class ExceptionDataSaver : IExceptionDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public ExceptionDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(CommonData.ISaveSettings settings, ExceptionData exceptionData)
        {
            if (exceptionData.Manager.GetState(exceptionData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(settings, exceptionData);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[bll].[CreateException_v2]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Int64);
                    id.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(id);

                    IDataParameter guid = DataUtil.CreateParameter(_providerFactory, "guid", DbType.Guid);
                    guid.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(guid);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(exceptionData.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "parentExceptionId", DbType.Int64, DataUtil.GetParameterValue(exceptionData.ParentExceptionId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "message", DbType.String, DataUtil.GetParameterValue(exceptionData.Message));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "typeName", DbType.String, DataUtil.GetParameterValue(exceptionData.TypeName));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "source", DbType.String, DataUtil.GetParameterValue(exceptionData.Source));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "appDomain", DbType.String, DataUtil.GetParameterValue(exceptionData.AppDomain));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "targetSite", DbType.String, DataUtil.GetParameterValue(exceptionData.TargetSite));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "stackTrace", DbType.String, DataUtil.GetParameterValue(exceptionData.StackTrace));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "data", DbType.String, DataUtil.GetParameterValue(exceptionData.Data));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "timestamp", DbType.DateTime2, DataUtil.GetParameterValue(exceptionData.CreateTimestamp));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "eventId", DbType.Guid, DataUtil.GetParameterValue(exceptionData.EventId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "category", DbType.String, DataUtil.GetParameterValue(exceptionData.Category));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "level", DbType.String, DataUtil.GetParameterValue(exceptionData.Level));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "parentExceptionGuid", DbType.Guid, DataUtil.GetParameterValue(exceptionData.ParentExceptionGuid));

                    _ = await command.ExecuteNonQueryAsync();
                    exceptionData.ExceptionId = (long)id.Value;
                    exceptionData.ExceptionGuid = (Guid)guid.Value;
                }
            }
        }
    }
}
