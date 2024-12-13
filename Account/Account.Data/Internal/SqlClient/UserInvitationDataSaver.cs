﻿using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.SqlClient
{
    public class UserInvitationDataSaver : IUserInvitationDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public UserInvitationDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISaveSettings settings, UserInvitationData userInvitationData)
        {
            if (userInvitationData.Manager.GetState(userInvitationData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(settings, userInvitationData);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[CreateUserInvitation]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter guid = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    guid.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(guid);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "accountGuid", DbType.Guid, userInvitationData.AccountId);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "emailAddressGuid", DbType.Guid, userInvitationData.EmailAddressId);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "status", DbType.Int16, userInvitationData.Status);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "expirationTimestamp", DbType.DateTime2, userInvitationData.ExpirationTimestamp);

                    _ = await command.ExecuteNonQueryAsync();
                    userInvitationData.UserInvitationId = (Guid)guid.Value;
                    userInvitationData.CreateTimestamp = (DateTime)timestamp.Value;
                    userInvitationData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Update(ISaveSettings settings, UserInvitationData userInvitationData)
        {
            if (userInvitationData.Manager.GetState(userInvitationData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(settings, userInvitationData);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[UpdateUserInvitation]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, userInvitationData.UserInvitationId);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "status", DbType.Int16, userInvitationData.Status);
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "expirationTimestamp", DbType.DateTime2, userInvitationData.ExpirationTimestamp);

                    _ = await command.ExecuteNonQueryAsync();
                    userInvitationData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
