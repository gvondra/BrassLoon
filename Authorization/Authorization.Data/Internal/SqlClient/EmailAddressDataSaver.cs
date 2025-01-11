using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public class EmailAddressDataSaver : DataSaverBase, IEmailAddressDataSaver
    {
        public EmailAddressDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task Create(CommonData.ISaveSettings settings, EmailAddressData data)
        {
            if (data.Manager.GetState(data) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(settings, data);
                using (DbCommand command = settings.Connection.CreateCommand())
                {
                    command.CommandText = "[blt].[CreateEmailAddress]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = settings.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "address", DbType.String, DataUtil.GetParameterValue(data.Address));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "addressHash", DbType.Binary, DataUtil.GetParameterValue(data.AddressHash));

                    _ = await command.ExecuteNonQueryAsync();
                    data.EmailAddressId = (Guid)id.Value;
                    data.CreateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
