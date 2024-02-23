using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;

namespace BrassLoon.Authorization.Data
{
    public class RoleDataSaver : DataSaverBase, IRoleDataSaver
    {
        public RoleDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task AddClientRole(ISqlTransactionHandler transactionHandler, Guid clientId, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[AddClientRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, DataUtil.GetParameterValue(clientId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddUserRole(ISqlTransactionHandler transactionHandler, Guid userId, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[AddUserRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.Guid, DataUtil.GetParameterValue(userId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, RoleData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[CreateRole]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

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

        public async Task RemoveClientRole(ISqlTransactionHandler transactionHandler, Guid clientId, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[RemoveClientRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "clientId", DbType.Guid, DataUtil.GetParameterValue(clientId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task RemoveUserRole(ISqlTransactionHandler transactionHandler, Guid userId, Guid roleId)
        {
            await _providerFactory.EstablishTransaction(transactionHandler);
            using (DbCommand command = transactionHandler.Connection.CreateCommand())
            {
                command.CommandText = "[blt].[RemoveUserRole]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

                DataUtil.AddParameter(_providerFactory, command.Parameters, "userId", DbType.Guid, DataUtil.GetParameterValue(userId));
                DataUtil.AddParameter(_providerFactory, command.Parameters, "roleId", DbType.Guid, DataUtil.GetParameterValue(roleId));

                _ = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, RoleData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, data);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[UpdateRole]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

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
