﻿using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public class TraceDataSaver : ITraceDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public TraceDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, TraceData traceData)
        {
            if (traceData.Manager.GetState(traceData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, traceData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bll].[CreateTrace]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Int64);
                    id.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(id);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(traceData.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "eventCode", DbType.AnsiString, DataUtil.GetParameterValue(traceData.EventCode));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "message", DbType.String, DataUtil.GetParameterValue(traceData.Message));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "data", DbType.String, DataUtil.GetParameterValue(traceData.Data));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "timestamp", DbType.DateTime2, DataUtil.GetParameterValue(traceData.CreateTimestamp));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "eventId", DbType.Guid, DataUtil.GetParameterValue(traceData.EventId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "category", DbType.String, DataUtil.GetParameterValue(traceData.Category));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "level", DbType.String, DataUtil.GetParameterValue(traceData.Level));

                    _ = await command.ExecuteNonQueryAsync();
                    traceData.TraceId = (long)id.Value;
                }
            }
        }
    }
}
