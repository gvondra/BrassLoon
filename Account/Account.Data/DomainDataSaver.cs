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
    public class DomainDataSaver : IDomainDataSaver
    {
        private ISqlDbProviderFactory _providerFactory;

        public DomainDataSaver(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task Create(ISqlTransactionHandler transactionHandler, DomainData domainData)
        {
            if (domainData.Manager.GetState(domainData) == DataState.New)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, domainData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[CreateDomain]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter id = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
                    id.Direction = ParameterDirection.Output;
                    command.Parameters.Add(id);

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "accountId", DbType.Guid, DataUtil.GetParameterValue(domainData.AccountGuid));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, DataUtil.GetParameterValue(domainData.Name));

                    await command.ExecuteNonQueryAsync();
                    domainData.DomainGuid = (Guid)id.Value;
                    domainData.CreateTimestamp = (DateTime)timestamp.Value;
                    domainData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }

        public async Task Update(ISqlTransactionHandler transactionHandler, DomainData domainData)
        {
            if (domainData.Manager.GetState(domainData) == DataState.Updated)
            {
                await _providerFactory.EstablishTransaction(transactionHandler, domainData);
                using (DbCommand command = transactionHandler.Connection.CreateCommand())
                {
                    command.CommandText = "[bla].[UpdateDomain]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transactionHandler.Transaction.InnerTransaction;

                    IDataParameter timestamp = DataUtil.CreateParameter(_providerFactory, "timestamp", DbType.DateTime2);
                    timestamp.Direction = ParameterDirection.Output;
                    command.Parameters.Add(timestamp);

                    DataUtil.AddParameter(_providerFactory, command.Parameters, "id", DbType.Guid, DataUtil.GetParameterValue(domainData.DomainGuid));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "name", DbType.String, DataUtil.GetParameterValue(domainData.Name));
                    DataUtil.AddParameter(_providerFactory, command.Parameters, "deleted", DbType.Boolean, DataUtil.GetParameterValue(domainData.Deleted));

                    await command.ExecuteNonQueryAsync();
                    domainData.UpdateTimestamp = (DateTime)timestamp.Value;
                }
            }
        }
    }
}
