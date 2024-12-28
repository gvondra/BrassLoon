using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data.Internal.SqlClient
{
    public class ItemDataFactory : IItemDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<ItemData> _dataFactory;

        public ItemDataFactory(ISqlDbProviderFactory providerFactory, IGenericDataFactory<ItemData> dataFactory)
        {
            _providerFactory = providerFactory;
            _dataFactory = dataFactory;
        }

        public async Task<ItemData> GetByCode(CommonData.ISettings settings, Guid domainId, string code)
        {
            List<IDataParameter> parameters = new List<IDataParameter>
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(_providerFactory, "code", DbType.AnsiString, code)
            };
            return (await _dataFactory.GetData(
                settings,
                _providerFactory,
                "[blc].[GetItemByCode]",
                () => new ItemData(),
                DataUtil.AssignDataStateManager,
                parameters)).FirstOrDefault();
        }

        public async Task<IEnumerable<string>> GetCodes(CommonData.ISettings settings, Guid domainId)
        {
            List<string> result = new List<string>();
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[blc].[GetItemCodes]";
                    command.CommandType = CommandType.StoredProcedure;
                    _ = command.Parameters.Add(
                        DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId));
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(await reader.GetFieldValueAsync<string>(0));
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return result;
        }
    }
}
