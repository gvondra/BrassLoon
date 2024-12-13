using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.SqlClient
{
    public class ClientCredentialDataSaver : IClientCredentialDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public ClientCredentialDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISaveSettings settings, ClientCredentialData clientCredentialData)
        {
            if (clientCredentialData.Manager.GetState(clientCredentialData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(settings, clientCredentialData);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[CreateClientCredential]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter guid = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    guid.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(guid);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, DataUtil.GetParameterValue(clientCredentialData.ClientId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "secret", DbType.Binary, DataUtil.GetParameterValue(clientCredentialData.Secret));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "isActive", DbType.Boolean, DataUtil.GetParameterValue(clientCredentialData.IsActive));

                    _ = await command.ExecuteNonQueryAsync();
                    clientCredentialData.ClientCredentialId = (Guid)guid.Value;
                    clientCredentialData.CreateTimestamp = (DateTime)timestamp.Value;
                    clientCredentialData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Update(ISaveSettings settings, ClientCredentialData clientCredentialData)
        {
            if (clientCredentialData.Manager.GetState(clientCredentialData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(settings, clientCredentialData);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[UpdateClientCredential]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(clientCredentialData.ClientCredentialId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "isActive", DbType.Boolean, DataUtil.GetParameterValue(clientCredentialData.IsActive));

                    _ = await command.ExecuteNonQueryAsync();
                    clientCredentialData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
