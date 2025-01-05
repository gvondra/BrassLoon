using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal.SqlClient
{
    public class PhoneDataSaver : DataSaverBase, IPhoneDataSaver
    {
        public PhoneDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory)
        { }

        public async Task Create(CommonData.ISaveSettings settings, PhoneData data)
        {
            if (data.Manager.GetState(data) != DataState.Unchanged)
            {
                await ProviderFactory.EstablishTransaction(settings, data);
                using DbCommand command = settings.Connection.CreateCommand();
                command.CommandText = "[blad].[CreatePhone]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = settings.Transaction.InnerTransaction;

                IDataParameter id = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid);
                id.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(id);

                IDataParameter timestamp = DataUtil.CreateParameter(ProviderFactory, "timestamp", DbType.DateTime2);
                timestamp.Direction = ParameterDirection.Output;
                _ = command.Parameters.Add(timestamp);

                DataUtil.AddParameter(ProviderFactory, command.Parameters, "domainId", DbType.Guid, DataUtil.GetParameterValue(data.DomainId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "keyId", DbType.Guid, DataUtil.GetParameterValue(data.KeyId));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "hash", DbType.Binary, DataUtil.GetParameterValue(data.Hash));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "initializationVector", DbType.Binary, DataUtil.GetParameterValue(data.InitializationVector));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "number", DbType.Binary, DataUtil.GetParameterValue(data.Number));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "countryCode", DbType.AnsiString, DataUtil.GetParameterValue(data.CountryCode));

                _ = await command.ExecuteNonQueryAsync();
                data.PhoneId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }
    }
}
