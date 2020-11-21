﻿using BrassLoon.Account.Data.Models;
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
        private IDbProviderFactory _providerFactory;

        public AccountDataSaver(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ITransactionHandler transactionHandler, Guid userGuid, AccountData accountData)
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

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "userGuid", DbType.Guid, userGuid);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, accountData.Name);

                    await command.ExecuteNonQueryAsync();
                    accountData.AccountGuid = (Guid)guid.Value;
                    accountData.CreateTimestamp = (DateTime)timestamp.Value;
                    accountData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Update(ITransactionHandler transactionHandler, AccountData accountData)
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

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "guid", DbType.Guid, accountData.AccountGuid);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, accountData.Name);

                    await command.ExecuteNonQueryAsync();
                    accountData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
