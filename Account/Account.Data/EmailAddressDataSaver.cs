using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public class EmailAddressDataSaver : IEmailAddressDataSaver
    {
        private readonly ISqlDbProviderFactory _providerFactory;

        public EmailAddressDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, EmailAddressData emailAddressData)
        {
            if (emailAddressData.Manager.GetState(emailAddressData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, emailAddressData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[CreateEmailAddress]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter guid = DataUtil.CreateParameter(_providerFactory, "guid", DbType.Guid);
                    guid.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(guid);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    _ = command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "address", DbType.String, emailAddressData.Address);

                    _ = await command.ExecuteNonQueryAsync();
                    emailAddressData.EmailAddressGuid = (Guid)guid.Value;
                    emailAddressData.CreateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
