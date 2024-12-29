using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.SqlClient
{
    public class ExceptionDataFactory : IExceptionDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<ExceptionData> _genericDataFactory;

        public ExceptionDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<ExceptionData>();
        }

        public async Task<ExceptionData> Get(ISqlSettings settings, long id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "exceptionId", DbType.Int64, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bll].[GetException]",
                () => new ExceptionData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<ExceptionData> GetInnerException(ISqlSettings settings, long id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Int64, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bll].[GetInnerException]",
                () => new ExceptionData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<ExceptionData>> GetTopBeforeTimestamp(ISqlSettings settings, Guid domainId, DateTime maxTimestamp)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId),
                DataUtil.CreateParameter(_providerFactory, "maxTimestamp", DbType.DateTime2, maxTimestamp)
            };
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bll].[GetTopExceptionBeforeTimestamp]",
                () => new ExceptionData(),
                DataUtil.AssignDataStateManager,
                parameters);
        }
    }
}
