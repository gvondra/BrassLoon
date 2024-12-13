using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.SqlClient
{
    public class AccountDataSaver : IAccountDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public AccountDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task AddUser(ISaveSettings settings, Guid userGuid, Guid accountGuid)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[bla].[UpdateAccountAddUser]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(_providerFactory, command.Parameters, "accountGuid", DbType.Guid, DataUtil.GetParameterValue(accountGuid));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "userGuid", DbType.Guid, DataUtil.GetParameterValue(userGuid));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Create(ISaveSettings settings, Guid userGuid, AccountData accountData)
        {
            if (accountData.Manager.GetState(accountData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(settings, accountData);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[CreateAccount]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter guid = DataUtil.CreateParameter(_providerFactory, "guid", DbType.Guid);
                    guid.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(guid);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "userGuid", DbType.Guid, DataUtil.GetParameterValue(userGuid));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, DataUtil.GetParameterValue(accountData.Name));

                    _ = await command.ExecuteNonQueryAsync();
                    accountData.AccountGuid = (Guid)guid.Value;
                    accountData.CreateTimestamp = (DateTime)timestamp.Value;
                    accountData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task RemoveUser(ISaveSettings settings, Guid userGuid, Guid accountGuid)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[bla].[UpdateAccountRemoveUser]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(_providerFactory, command.Parameters, "accountGuid", DbType.Guid, DataUtil.GetParameterValue(accountGuid));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "userGuid", DbType.Guid, DataUtil.GetParameterValue(userGuid));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(ISaveSettings settings, AccountData accountData)
        {
            if (accountData.Manager.GetState(accountData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(settings, accountData);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[UpdateAccount]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "guid", DbType.Guid, DataUtil.GetParameterValue(accountData.AccountGuid));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, DataUtil.GetParameterValue(accountData.Name));

                    _ = await command.ExecuteNonQueryAsync();
                    accountData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task UpdateLocked(ISaveSettings settings, Guid accountId, bool locked)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[bla].[UpdateAccountLocked]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(_providerFactory, command.Parameters, "guid", DbType.Guid, DataUtil.GetParameterValue(accountId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "locked", DbType.Boolean, DataUtil.GetParameterValue(locked));

                _ = await command.ExecuteNonQueryAsync();
            }
        }
    }
}
