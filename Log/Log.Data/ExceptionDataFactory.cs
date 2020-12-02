using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public class ExceptionDataFactory : IExceptionDataFactory
    {
        private IDbProviderFactory _providerFactory;
        private GenericDataFactory<ExceptionData> _genericDataFactory;

        public ExceptionDataFactory(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<ExceptionData>();
        }

        public async Task<ExceptionData> GetInnerException(ISettings settings, long id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Int64, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bll].[GetInnerException]",
                () => new ExceptionData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }
                )).FirstOrDefault();
        }
    }
}
