using BrassLoon.CommonAPI;
using BrassLoon.Log.Framework;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LogRPC.Protos;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogRPC.Services
{
    public class ExceptionService : Protos.ExceptionService.ExceptionServiceBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly SettingsFactory _settingsFactory;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IEventIdFactory _eventIdFactory;
        private readonly IExceptionFactory _exceptionFactory;
        private readonly IExceptionSaver _exceptionSaver;

        public ExceptionService(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IMetaDataProcessor metaDataProcessor,
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IEventIdFactory eventIdFactory,
            IExceptionFactory exceptionFactory,
            IExceptionSaver exceptionSaver)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
            _metaDataProcessor = metaDataProcessor;
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _eventIdFactory = eventIdFactory;
            _exceptionFactory = exceptionFactory;
            _exceptionSaver = exceptionSaver;
        }

        public async override Task<Empty> Create(LogException request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(request.DomainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing {nameof(LogException.DomainId)} parameter value");
                Guid domainId = Guid.Parse(request.DomainId);
                if (domainId.Equals(Guid.Empty))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing {nameof(LogException.DomainId)} parameter value");
                if (!(await _domainAcountAccessVerifier.HasAccess(
                    _settings.Value,
                    domainId,
                    _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders))))
                {
                    throw new RpcException(new Status(StatusCode.Unauthenticated, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                IEventId eventId = await GetInnerEventId(settings, domainId, request.EventId);
                List<IException> allExceptions = new List<IException>();
                IException innerException = await Map(
                    settings,
                    request,
                    domainId,
                    request.CreateTimestamp != null ? request.CreateTimestamp.ToDateTime() : default(DateTime?),
                    allExceptions);
                await _exceptionSaver.Create(settings, allExceptions.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new RpcException(new Status(StatusCode.Internal, "Internal System Error"), ex.Message);
            }
            return new Empty();
        }

        private async Task<IException> Map(
            CoreSettings settings,
            LogException exception,
            Guid domainId,
            DateTime? timestamp,
            List<IException> allExceptions,
            IException parentException = null)
        {
            IEventId innerEventId = await GetInnerEventId(settings, domainId, exception.EventId);
            IException innerException = _exceptionFactory.Create(
                domainId,
                timestamp,
                parentException,
                innerEventId);
            innerException.AppDomain = exception.AppDomain;
            innerException.Category = exception.Category;
            innerException.Data = GetExceptionData(exception.Data);
            innerException.Level = exception.Level;
            innerException.Message = exception.Message;
            innerException.Source = exception.Source;
            innerException.StackTrace = exception.StackTrace;
            innerException.TargetSite = exception.TargetSite;
            innerException.TypeName = exception.TypeName;
            allExceptions.Add(innerException);
            if (exception.InnerException != null)
                await Map(settings, exception.InnerException, domainId, timestamp, allExceptions, innerException);
            return innerException;
        }

        private object GetExceptionData(Google.Protobuf.Collections.MapField<string, string> map)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (map != null)
            {
                foreach (string key in map.Keys)
                {
                    result[key] = map[key];
                }
            }
            return result;
        }

        private async Task<IEventId> GetInnerEventId(CoreSettings settings, Guid domainId, Protos.EventId eventId)
        {
            IEventId innerEventId = null;
            if (eventId != null && (eventId.Id != 0 || !string.IsNullOrEmpty(eventId.Name)))
            {
                innerEventId = (await _eventIdFactory.GetByDomainId(settings, domainId))
                                .FirstOrDefault(i => i.Id == eventId.Id && string.Equals(i.Name, eventId.Name, StringComparison.OrdinalIgnoreCase))
                                ?? _eventIdFactory.Create(domainId, eventId.Id, eventId.Name);
            }
            return innerEventId;
        }
    }
}
