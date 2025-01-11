using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public class SigningKeyDataSaver : DataSaverBase, ISigningKeyDataSaver
    {
        public SigningKeyDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(CommonData.ISaveSettings settings, SigningKeyData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(settings, data);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[CreateSigningKey]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "keyVaultKey", DbType.Guid, DataUtil.GetParameterValue(data.KeyVaultKey));
                    AddCommonParameters(command.Parameters, data);

                    _ = await command.ExecuteNonQueryAsync();
                    data.SigningKeyId = (Guid)id.Value;
                    data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                    data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }

        public async Task Update(CommonData.ISaveSettings settings, SigningKeyData data)
        {
            if (data.Manager.GetState(data) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(settings, data);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[UpdateSigningKey]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(data.SigningKeyId));
                    AddCommonParameters(command.Parameters, data);

                    _ = await command.ExecuteNonQueryAsync();
                    data.UpdateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
                }
            }
        }

        private void AddCommonParameters(IList commandParameters, SigningKeyData data) => DataUtil.AddParameter(_providerFactory, commandParameters, "isActive", DbType.Boolean, DataUtil.GetParameterValue(data.IsActive));
    }
}
