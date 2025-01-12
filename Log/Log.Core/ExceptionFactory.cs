using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class ExceptionFactory : IExceptionFactory
    {
        private readonly IExceptionDataFactory _dataFactory;
        private readonly IExceptionDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;

        public ExceptionFactory(
            IExceptionDataFactory dataFactory,
            IExceptionDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
        }

        private Exception Create(ExceptionData data) => new Exception(data, _dataFactory, _dataSaver, this);
        private Exception Create(ExceptionData data, IEventId eventId) => new Exception(data, _dataFactory, _dataSaver, this, eventId);

        public IException Create(Guid domainId, DateTime? createTimestamp, IEventId eventId = null) => Create(domainId, createTimestamp, parentException: null, eventId: eventId);

        public IException Create(Guid domainId, DateTime? createTimestamp, IException parentException, IEventId eventId = null)
        {
            if (!createTimestamp.HasValue)
                createTimestamp = DateTime.UtcNow;
            createTimestamp = createTimestamp.Value.ToUniversalTime();
            Exception result = Create(
                new ExceptionData() { DomainId = domainId, CreateTimestamp = createTimestamp.Value },
                eventId);
            result.ParentException = parentException;
            return result;
        }

        public async Task<IException> Get(ISettings settings, Guid id)
        {
            IException result = null;
            ExceptionData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = Create(data);
            return result;
        }

        public async Task<IEnumerable<IException>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, DateTime maxTimestamp)
        {
            return (await _dataFactory.GetTopBeforeTimestamp(_settingsFactory.CreateData(settings), domainId, maxTimestamp.ToUniversalTime()))
                .Select<ExceptionData, IException>(Create)
                ;
        }
    }
}
