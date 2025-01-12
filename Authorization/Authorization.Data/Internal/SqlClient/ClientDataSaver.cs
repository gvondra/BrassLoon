using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public class ClientDataSaver : DataSaverBase, IClientDataSaver
    {
        public ClientDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(CommonData.ISaveSettings settings, ClientData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(settings, data);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[CreateClient]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "secretKey", DbType.Guid, DataUtil.GetParameterValue(data.SecretKey));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "secretSalt", DbType.Binary, DataUtil.GetParameterValue(data.SecretSalt));
                    AddCommonParameters(command.Parameters, data);

                    _ = await command.ExecuteNonQueryAsync();
                    data.ClientId = (Guid)id.Value;
                    data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                    data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }

        public async Task Update(CommonData.ISaveSettings settings, ClientData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(settings, data);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[UpdateClient]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.ClientId));
                    AddCommonParameters(command.Parameters, data);

                    _ = await command.ExecuteNonQueryAsync();
                    data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }

        public async Task AddRole(CommonData.ISaveSettings settings, ClientData data, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[AddClientRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, DataUtil.GetParameterValue(data.ClientId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task RemoveRole(CommonData.ISaveSettings settings, ClientData data, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[RemoveClientRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, DataUtil.GetParameterValue(data.ClientId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        private void AddCommonParameters(IList commandParameters, ClientData data)
        {
            DataUtil.AddParameter(_providerFactory, commandParameters, "name", DbType.String, DataUtil.GetParameterValue(data.Name));
            DataUtil.AddParameter(_providerFactory, commandParameters, "isActive", DbType.Boolean, DataUtil.GetParameterValue(data.IsActive));
            DataUtil.AddParameter(_providerFactory, commandParameters, "userEmailAddressId", DbType.Guid, DataUtil.GetParameterValue(data.UserEmailAddressId));
            DataUtil.AddParameter(_providerFactory, commandParameters, "userName", DbType.String, DataUtil.GetParameterValue(data.UserName));
        }
    }
}
