using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public class PurgeDataFactory : IPurgeDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly IGenericDataFactory<PurgeData> _genericDataFactory;

        public PurgeDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<PurgeData>();
        }

        public Task<PurgeData> GetExceptionByTargetId(ISqlSettings settings, long targetId)
        {
            return GetByTargetId(settings, targetId, "[bll].[GetExceptionPurgeByTargetId]");
        }

        public Task<PurgeData> GetMetricByTargetId(ISqlSettings settings, long targetId)
        {
            return GetByTargetId(settings, targetId, "[bll].[GetMetricPurgeByTargetId]");
        }

        public Task<PurgeData> GetTraceByTargetId(ISqlSettings settings, long targetId)
        {
            return GetByTargetId(settings, targetId, "[bll].[GetTracePurgeByTargetId]");
        }

        private async Task<PurgeData> GetByTargetId(ISqlSettings settings, long targetId, string procedureName)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "targetId", DbType.Int64, targetId)
            };
            return (await _genericDataFactory.GetData(
                settings, 
                _providerFactory, 
                procedureName, 
                () => new PurgeData(), 
                DataUtil.AssignDataStateManager, 
                parameters
                ))
                .FirstOrDefault();
        }
    }
}
