using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal
{
    public class AddressDataSaver : DataSaverBase, IAddressDataSaver
    {
        public AddressDataSaver(IDbProviderFactory providerFactory) : base(providerFactory)
        { }

        public async Task Create(ISqlTransactionHandler transactionHandler, AddressData data)
        {
            if (data.Manager.GetState(data) != DataState.Unchanged)
            {
                await ProviderFactory.EstablishTransaction(transactionHandler, data);
                using DbCommand command = transactionHandler.Connection.CreateCommand();
                command.CommandText = "[blad].[CreateAddress]";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transactionHandler.Transaction.InnerTransaction;

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
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "attention", DbType.Binary, DataUtil.GetParameterValue(data.Attention));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "addressee", DbType.Binary, DataUtil.GetParameterValue(data.Addressee));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "delivery", DbType.Binary, DataUtil.GetParameterValue(data.Delivery));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "city", DbType.Binary, DataUtil.GetParameterValue(data.City));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "territory", DbType.Binary, DataUtil.GetParameterValue(data.Territory));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "postalCode", DbType.Binary, DataUtil.GetParameterValue(data.PostalCode));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "country", DbType.Binary, DataUtil.GetParameterValue(data.Country));
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "county", DbType.Binary, DataUtil.GetParameterValue(data.County));

                _ = await command.ExecuteNonQueryAsync();
                data.AddressId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }
    }
}
