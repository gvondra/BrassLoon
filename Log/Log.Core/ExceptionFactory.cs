using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class ExceptionFactory : IExceptionFactory
    {
        private readonly IExceptionDataFactory _dataFactory;
        private readonly IExceptionDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;

        public ExceptionFactory(IExceptionDataFactory dataFactory,
            IExceptionDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
        }

        public IException Create(Guid domainId)
        {
            return Create(domainId, null);
        }

        public IException Create(Guid domainId, IException parentException)
        {
            return new Exception(
                new ExceptionData() { DomainId = domainId },
                _dataSaver,
                this
                );
        }

        public async Task<IException> GetInnerException(ISettings settings, long id)
        {
            IException exception = null;
            ExceptionData data = await _dataFactory.GetInnerException(_settingsFactory.CreateData(settings), id);
            if (data != null)
                exception = new Exception(data, _dataSaver, this);
            return exception;
        }
    }
}
