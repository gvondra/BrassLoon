using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public class UserDataSaver : DataSaverBase, IUserDataSaver
    {
        public UserDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(CommonData.ISaveSettings settings, UserData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(settings, data);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[CreateUser]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "referenceId", DbType.AnsiString, DataUtil.GetParameterValue(data.ReferenceId));
                    AddCommonParameters(command.Parameters, data);

                    _ = await command.ExecuteNonQueryAsync();
                    data.UserId = (Guid)id.Value;
                    data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                    data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }

        public async Task Update(CommonData.ISaveSettings settings, UserData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(settings, data);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[UpdateUser]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.UserId));
                    AddCommonParameters(command.Parameters, data);

                    _ = await command.ExecuteNonQueryAsync();
                    data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }

        public async Task AddRole(CommonData.ISaveSettings settings, UserData data, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[AddUserRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.Guid, DataUtil.GetParameterValue(data.UserId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task RemoveRole(CommonData.ISaveSettings settings, UserData data, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[RemoveUserRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.Guid, DataUtil.GetParameterValue(data.UserId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        private void AddCommonParameters(IList commandParameters, UserData data)
        {
            DataUtil.AddParameter(_providerFactory, commandParameters, "emailAddressId", DbType.Guid, DataUtil.GetParameterValue(data.EmailAddressId));
            DataUtil.AddParameter(_providerFactory, commandParameters, "name", DbType.String, DataUtil.GetParameterValue(data.Name));
        }
    }
}
