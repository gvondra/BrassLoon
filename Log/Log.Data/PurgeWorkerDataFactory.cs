using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public class PurgeWorkerDataFactory : IPurgeWorkerDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<PurgeWorkerData> _genericDataFactory;

        public PurgeWorkerDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<PurgeWorkerData>();
        }

        public async Task<Guid?> ClaimPurgeWorker(ISqlSettings settings)
        {
            Guid? result = null;
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid);
            parameter.Direction = ParameterDirection.Output;

            using (DbConnection connection = await _providerFactory.OpenConnection(settings))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[bll].[ClaimPurgeWorker]";
                    command.CommandType = CommandType.StoredProcedure;
                    _ = command.Parameters.Add(parameter);
                    _ = await command.ExecuteNonQueryAsync();
                    if (parameter.Value != null && parameter.Value != DBNull.Value)
                        result = (Guid)parameter.Value;
                }
            }
            return result;
        }

        public async Task<PurgeWorkerData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "purgeWorkerId", DbType.Guid, id),
            };

            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bll].[GetPurgeWorker]",
                () => new PurgeWorkerData(),
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<PurgeWorkerData>> GetAll(ISqlSettings settings)
        {
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bll].[GetAllPurgeWorker]",
                () => new PurgeWorkerData(),
                DataUtil.AssignDataStateManager);
        }
    }
}
