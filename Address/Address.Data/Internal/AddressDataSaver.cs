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
            if (data.Manager.GetState(data) == DataState.New)
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
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "hash", DbType.Binary, DataUtil.GetParameterValue(data.Hash));

                _ = await command.ExecuteNonQueryAsync();
                data.AddressId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }
    }
}
