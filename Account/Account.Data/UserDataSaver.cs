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
    public class UserDataSaver : IUserDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public UserDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, UserData userData)
        {
            if (userData.Manager.GetState(userData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, userData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[CreateUser]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter guid = DataUtil.CreateParameter(_providerFactory, "guid", DbType.Guid);
                    guid.Direction = ParameterDirection.Output;
                    command.Parameters.Add(guid);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "referenceId", DbType.AnsiString, userData.ReferenceId);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "emailAddressGuid", DbType.Guid, userData.EmailAddressGuid);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.AnsiString, userData.Name);

                    await command.ExecuteNonQueryAsync();
                    userData.UserGuid = (Guid)guid.Value;
                    userData.CreateTimestamp = (DateTime)timestamp.Value;
                    userData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, UserData userData)
        {
            if (userData.Manager.GetState(userData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, userData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[UpdateUser]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "guid", DbType.Guid, userData.UserGuid);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "emailAddressGuid", DbType.Guid, userData.EmailAddressGuid);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "roles", DbType.Int16, userData.Roles);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.AnsiString, userData.Name);

                    await command.ExecuteNonQueryAsync();
                    userData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
