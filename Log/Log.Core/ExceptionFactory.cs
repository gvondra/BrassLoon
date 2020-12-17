using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IException Create(Guid domainId, DateTime? createTimestamp)
        {
            return Create(domainId, createTimestamp, null);
        }

        public IException Create(Guid domainId, DateTime? createTimestamp, IException parentException)
        {
            if (!createTimestamp.HasValue)
                createTimestamp = DateTime.UtcNow;
            createTimestamp = createTimestamp.Value.ToUniversalTime();
            return new Exception(
                new ExceptionData() { DomainId = domainId, CreateTimestamp = createTimestamp.Value },
                _dataSaver,
                this
                );
        }

        public async Task<IException> Get(ISettings settings, long id)
        {
            IException result = null;
            ExceptionData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = new Exception(data, _dataSaver, this);
            return result;
        }

        public async Task<IException> GetInnerException(ISettings settings, long id)
        {
            IException exception = null;
            ExceptionData data = await _dataFactory.GetInnerException(_settingsFactory.CreateData(settings), id);
            if (data != null)
                exception = new Exception(data, _dataSaver, this);
            return exception;
        }

        public async Task<IEnumerable<IException>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, DateTime maxTimestamp)
        {
            return (await _dataFactory.GetTopBeforeTimestamp(_settingsFactory.CreateData(settings), domainId, maxTimestamp.ToUniversalTime()))
                .Select<ExceptionData, IException>(data => new Exception(data, _dataSaver, this))
                ;
        }
    }
}
