using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public class LookupDataSaver : ILookupDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public LookupDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, LookupData lookupData)
        {
            if (lookupData.Manager.GetState(lookupData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, lookupData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[blc].[CreateLookup]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(lookupData.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "code", DbType.AnsiString, DataUtil.GetParameterValue(lookupData.Code));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "data", DbType.AnsiString, DataUtil.GetParameterValue(lookupData.Data));

                    _ = await command.ExecuteNonQueryAsync();
                    lookupData.LookupId = (Guid)id.Value;
                    lookupData.CreateTimestamp = (DateTime)timestamp.Value;
                    lookupData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task DeleteByCode(ISqlTransactionHandler transactionHandler, Guid domainId, string code)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[blc].[DeleteLookupByCode]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(domainId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "code", DbType.AnsiString, DataUtil.GetParameterValue(code));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, LookupData lookupData)
        {
            if (lookupData.Manager.GetState(lookupData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, lookupData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[blc].[UpdateLookup]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(lookupData.LookupId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "code", DbType.AnsiString, DataUtil.GetParameterValue(lookupData.Code));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "data", DbType.AnsiString, DataUtil.GetParameterValue(lookupData.Data));

                    _ = await command.ExecuteNonQueryAsync();
                    lookupData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
