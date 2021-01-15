using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public class ItemDataFactory : IItemDataFactory
    {
        private ISqlDbProviderFactory _providerFactory;
        private GenericDataFactory<ItemData> _genericDataFactory;

        public ItemDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<ItemData>();
        }

        public async Task<ItemData> GetByCode(ISqlSettings settings, Guid domainId, string code)
        {
            List<IDataParameter> parameters = new List<IDataParameter>
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(_providerFactory, "code", DbType.AnsiString, code)
            };
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blc].[GetItemByCode]",
                () => new ItemData(),
                DataUtil.AssignDataStateManager,
                parameters
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<string>> GetCodes(ISqlSettings settings, Guid domainId)
        {
            List<string> result = new List<string>();
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[blc].[GetItemCodes]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(
                        DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId)
                        );
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(await reader.GetFieldValueAsync<string>(0));
                        }
                    }
                }
                connection.Close();
            }
            return result;
        }
    }
}
