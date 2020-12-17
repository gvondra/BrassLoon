using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public class EmailAddressDataSaver : IEmailAddressDataSaver
    {
        private ISqlDbProviderFactory _providerFactory;

        public EmailAddressDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ITransactionHandler transactionHandler, EmailAddressData emailAddressData)
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
                    command.Parameters.Add(guid);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "address", DbType.String, emailAddressData.Address);

                    await command.ExecuteNonQueryAsync();
                    emailAddressData.EmailAddressGuid = (Guid)guid.Value;
                    emailAddressData.CreateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
