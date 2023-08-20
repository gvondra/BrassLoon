using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public class ClientDataSaver : IClientDataSaver
    {
        private ISqlDbProviderFactory _providerFactory;

        public ClientDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, ClientData clientData)
        {
            if (clientData.Manager.GetState(clientData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, clientData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[CreateClient]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter guid = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    guid.Direction = ParameterDirection.Output;
                    command.Parameters.Add(guid);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "accountId", DbType.Guid, DataUtil.GetParameterValue(clientData.AccountId));
                    AddCommonParameters(command.Parameters, clientData);

                    await command.ExecuteNonQueryAsync();
                    clientData.ClientId = (Guid)guid.Value;
                    clientData.CreateTimestamp = (DateTime)timestamp.Value;
                    clientData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, ClientData clientData)
        {
            if (clientData.Manager.GetState(clientData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, clientData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[UpdateClient]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(clientData.ClientId));
                    AddCommonParameters(command.Parameters, clientData);

                    await command.ExecuteNonQueryAsync();
                    clientData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        private void AddCommonParameters(IList commandParameters, ClientData data)
        {
            DataUtil.AddParameter(_providerFactory, commandParameters, "name", DbType.String, DataUtil.GetParameterValue(data.Name));
            DataUtil.AddParameter(_providerFactory, commandParameters, "secretType", DbType.Int16, DataUtil.GetParameterValue(data.SecretType));
            DataUtil.AddParameter(_providerFactory, commandParameters, "secretKey", DbType.Guid, DataUtil.GetParameterValue(data.SecretKey));
            DataUtil.AddParameter(_providerFactory, commandParameters, "secretSalt", DbType.Binary, DataUtil.GetParameterValue(data.SecretSalt));
            DataUtil.AddParameter(_providerFactory, commandParameters, "isActive", DbType.Boolean, DataUtil.GetParameterValue(data.IsActive));
        }
    }
}
