using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public class RoleDataSaver : DataSaverBase, IRoleDataSaver
    {
        public RoleDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task AddClientRole(CommonData.ISaveSettings settings, Guid clientId, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[AddClientRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, DataUtil.GetParameterValue(clientId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddUserRole(CommonData.ISaveSettings settings, Guid userId, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[AddUserRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.Guid, DataUtil.GetParameterValue(userId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Create(CommonData.ISaveSettings settings, RoleData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(settings, data);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[CreateRole]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "policyName", DbType.AnsiString, DataUtil.GetParameterValue(data.PolicyName));
                    AddCommonParameters(command.Parameters, data);

                    _ = await command.ExecuteNonQueryAsync();
                    data.RoleId = (Guid)id.Value;
                    data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                    data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }

        public async Task RemoveClientRole(CommonData.ISaveSettings settings, Guid clientId, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[RemoveClientRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, DataUtil.GetParameterValue(clientId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task RemoveUserRole(CommonData.ISaveSettings settings, Guid userId, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(settings);
            using (DbCommand command = settings.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[RemoveUserRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.Guid, DataUtil.GetParameterValue(userId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(CommonData.ISaveSettings settings, RoleData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(settings, data);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[UpdateRole]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.RoleId));
                    AddCommonParameters(command.Parameters, data);

                    _ = await command.ExecuteNonQueryAsync();
                    data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }

        private void AddCommonParameters(IList commandParameters, RoleData data)
        {
            DataUtil.AddParameter(_providerFactory, commandParameters, "name", DbType.AnsiString, DataUtil.GetParameterValue(data.Name));
            DataUtil.AddParameter(_providerFactory, commandParameters, "isActive", DbType.Boolean, DataUtil.GetParameterValue(data.IsActive));
            DataUtil.AddParameter(_providerFactory, commandParameters, "comment", DbType.String, DataUtil.GetParameterValue(data.Comment));
        }
    }
}
