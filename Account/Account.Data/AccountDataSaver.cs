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
    public class AccountDataSaver : IAccountDataSaver
    {
        private ISqlDbProviderFactory _providerFactory;

        public AccountDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task AddUser(ISqlTransactionHandler transactionHandler, Guid userGuid, Guid accountGuid)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[bla].[UpdateAccountAddUser]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                command.Parameters.Add(timestamp);

                DataUtil.AddParameter(_providerFactory, command.Parameters, "accountGuid", DbType.Guid, DataUtil.GetParameterValue(accountGuid));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "userGuid", DbType.Guid, DataUtil.GetParameterValue(userGuid));

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, Guid userGuid, AccountData accountData)
        {
            if (accountData.Manager.GetState(accountData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, accountData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[CreateAccount]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter guid = DataUtil.CreateParameter(_providerFactory, "guid", DbType.Guid);
                    guid.Direction = ParameterDirection.Output;
                    command.Parameters.Add(guid);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "userGuid", DbType.Guid, DataUtil.GetParameterValue(userGuid));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, DataUtil.GetParameterValue(accountData.Name));

                    await command.ExecuteNonQueryAsync();
                    accountData.AccountGuid = (Guid)guid.Value;
                    accountData.CreateTimestamp = (DateTime)timestamp.Value;
                    accountData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task RemoveUser(ISqlTransactionHandler transactionHandler, Guid userGuid, Guid accountGuid)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[bla].[UpdateAccountRemoveUser]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                command.Parameters.Add(timestamp);

                DataUtil.AddParameter(_providerFactory, command.Parameters, "accountGuid", DbType.Guid, DataUtil.GetParameterValue(accountGuid));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "userGuid", DbType.Guid, DataUtil.GetParameterValue(userGuid));

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, AccountData accountData)
        {
            if (accountData.Manager.GetState(accountData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, accountData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[UpdateAccount]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "guid", DbType.Guid, DataUtil.GetParameterValue(accountData.AccountGuid));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, DataUtil.GetParameterValue(accountData.Name));

                    await command.ExecuteNonQueryAsync();
                    accountData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
