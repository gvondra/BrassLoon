using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.SqlClient
{
    public class AccountDataFactory : IAccountDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<AccountData> _genericDataFactory;

        public AccountDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<AccountData>();
        }

        public async Task<AccountData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "guid", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetAccount]",
                () => new AccountData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })).FirstOrDefault();
        }

        public async Task<IEnumerable<Guid>> GetAccountIdsByUserId(ISqlSettings settings, Guid userId)
        {
            List<Guid> result = new List<Guid>();
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "userGuid", DbType.Guid, userId);
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[bla].[GetAccountGuidByUserGuid]";
                    command.CommandType = CommandType.StoredProcedure;
                    _ = command.Parameters.Add(parameter);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(await reader.GetFieldValueAsync<Guid>(0));
                        }
                        await reader.CloseAsync();
                    }
                }
                await connection.CloseAsync();
            }
            return result;
        }

        public async Task<IEnumerable<AccountData>> GetByUserId(ISqlSettings settings, Guid userId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "userGuid", DbType.Guid, userId);
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetAccountByUserGuid]",
                () => new AccountData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter });
        }
    }
}
