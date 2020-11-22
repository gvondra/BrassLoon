using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public class ExceptionDataSaver : IExceptionDataSaver
    {
        private IDbProviderFactory _providerFactory;

        public ExceptionDataSaver(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ITransactionHandler transactionHandler, ExceptionData exceptionData)
        {
            if (exceptionData.Manager.GetState(exceptionData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, exceptionData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bll].[CreateException]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Int64);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(exceptionData.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "parentExceptionId", DbType.Int64, DataUtil.GetParameterValue(exceptionData.ParentExceptionId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "message", DbType.String, DataUtil.GetParameterValue(exceptionData.Message));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "typeName", DbType.String, DataUtil.GetParameterValue(exceptionData.TypeName));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "source", DbType.String, DataUtil.GetParameterValue(exceptionData.Source));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "appDomain", DbType.String, DataUtil.GetParameterValue(exceptionData.AppDomain));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "targetSite", DbType.String, DataUtil.GetParameterValue(exceptionData.TargetSite));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "stackTrace", DbType.String, DataUtil.GetParameterValue(exceptionData.StackTrace));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "data", DbType.String, DataUtil.GetParameterValue(exceptionData.Data));

                    await command.ExecuteNonQueryAsync();
                    exceptionData.ExceptionId = (long)id.Value;
                    exceptionData.CreateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
