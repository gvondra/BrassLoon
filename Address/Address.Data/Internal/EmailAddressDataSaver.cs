using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient;
using System.Data.Common;
using System.Data;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal
{
    public class EmailAddressDataSaver : DataSaverBase, IEmailAddressDataSaver
    {
        public EmailAddressDataSaver(IDbProviderFactory providerFactory)
            : base(providerFactory)
        { }

        public async Task Create(ISqlTransactionHandler transactionHandler, EmailAddressData data)
        {
            if (data.Manager.GetState(data) != DataState.Unchanged)
            {
                await ProviderFactory.EstablishTransaction(transactionHandler, data);
                using DbCommand command = transactionHandler.Connection.CreateCommand();
                command.CommandText = "[blad].[CreateEmailAddress]";
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
                DataUtil.AddParameter(ProviderFactory, command.Parameters, "address", DbType.Binary, DataUtil.GetParameterValue(data.Address));

                _ = await command.ExecuteNonQueryAsync();
                data.EmailAddressId = (Guid)id.Value;
                data.CreateTimestamp = DateTime.SpecifyKind((DateTime)timestamp.Value, DateTimeKind.Utc);
            }
        }
    }
}
