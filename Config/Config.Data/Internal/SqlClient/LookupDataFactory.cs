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
    public class LookupDataFactory : ILookupDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<LookupData> _genericDataFactory;

        public LookupDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<LookupData>();
        }

        public async Task<LookupData> GetByCode(ISettings settings, Guid domainId, string code)
        {
            List<IDataParameter> parameters = new List<IDataParameter>
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(_providerFactory, "code", DbType.AnsiString, code)
            };
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blc].[GetLookupByCode]",
                () => new LookupData(),
                DataUtil.AssignDataStateManager,
                parameters)).FirstOrDefault();
        }

        public async Task<IEnumerable<string>> GetCodes(ISettings settings, Guid domainId)
        {
            List<string> result = new List<string>();
            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[blc].[GetLookupCodes]";
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
