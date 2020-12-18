using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public class ClientCredentialDataSaver : IClientCredentialDataSaver
    {
        private ISqlDbProviderFactory _providerFactory;

        public ClientCredentialDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, ClientCredentialData clientCredentialData)
        {
            if (clientCredentialData.Manager.GetState(clientCredentialData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, clientCredentialData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[CreateClientCredential]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter guid = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    guid.Direction = ParameterDirection.Output;
                    command.Parameters.Add(guid);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, DataUtil.GetParameterValue(clientCredentialData.ClientId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "secret", DbType.Binary, DataUtil.GetParameterValue(clientCredentialData.Secret));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "isActive", DbType.Boolean, DataUtil.GetParameterValue(clientCredentialData.IsActive));

                    await command.ExecuteNonQueryAsync();
                    clientCredentialData.ClientCredentialId = (Guid)guid.Value;
                    clientCredentialData.CreateTimestamp = (DateTime)timestamp.Value;
                    clientCredentialData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, ClientCredentialData clientCredentialData)
        {
            if (clientCredentialData.Manager.GetState(clientCredentialData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, clientCredentialData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[UpdateClientCredential]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(clientCredentialData.ClientCredentialId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "isActive", DbType.Boolean, DataUtil.GetParameterValue(clientCredentialData.IsActive));

                    await command.ExecuteNonQueryAsync();
                    clientCredentialData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
