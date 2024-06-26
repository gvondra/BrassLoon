﻿using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;

namespace BrassLoon.WorkTask.Data.Internal
{
    public class PurgeWorkerDataFactory : DataFactoryBase<PurgeWorkerData>, IPurgeWorkerDataFactory
    {
        public PurgeWorkerDataFactory(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task<Guid?> ClaimPurgeWorker(ISqlSettings settings)
        {
            Guid? result = null;
            IDataParameter parameter = DataUtil.CreateParameter(ProviderFactory, "id", DbType.Guid);
            parameter.Direction = ParameterDirection.Output;

            using (DbConnection connection = await ProviderFactory.OpenConnection(settings))
            {
                using DbCommand command = connection.CreateCommand();
                command.CommandText = "[blwt].[ClaimPurgeWorker]";
                command.CommandType = CommandType.StoredProcedure;
                _ = command.Parameters.Add(parameter);
                _ = await command.ExecuteNonQueryAsync();
                if (parameter.Value != null && parameter.Value != DBNull.Value)
                    result = (Guid)parameter.Value;
            }
            return result;
        }

        public async Task<PurgeWorkerData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(ProviderFactory, "purgeWorkerId", DbType.Guid, id),
            };

            return (await GenericDataFactory.GetData(
                settings,
                ProviderFactory,
                "[blwt].[GetPurgeWorker]",
                () => new PurgeWorkerData(),
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }
    }
}
