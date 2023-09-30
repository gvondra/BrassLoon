using BrassLoon.CommonAPI;
using BrassLoon.Log.Framework;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LogRPC.Protos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogRPC.Services
{
    public class TraceService : Protos.TraceService.TraceServiceBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly SettingsFactory _settingsFactory;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IEventIdFactory _eventIdFactory;
        private readonly ITraceFactory _traceFactory;
        private readonly ITraceSaver _traceSaver;

        public TraceService(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IMetaDataProcessor metaDataProcessor,
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IEventIdFactory eventIdFactory,
            ITraceFactory traceFactory,
            ITraceSaver traceSaver)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
            _metaDataProcessor = metaDataProcessor;
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _eventIdFactory = eventIdFactory;
            _traceFactory = traceFactory;
            _traceSaver = traceSaver;
        }

        [Authorize("BL:AUTH")]
        public override async Task<Empty> Create(IAsyncStreamReader<Trace> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                try
                {
                    if (string.IsNullOrEmpty(requestStream.Current.DomainId))
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing {nameof(Trace.DomainId)} parameter value");
                    Guid domainId = Guid.Parse(requestStream.Current.DomainId);
                    if (domainId.Equals(Guid.Empty))
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing {nameof(Trace.DomainId)} parameter value");
                    if (!(await _domainAcountAccessVerifier.HasAccess(
                        _settingsFactory.CreateAccount(_settings.Value, _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders)),
                        domainId,
                        _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders))))
                    {
                        throw new RpcException(new Status(StatusCode.Unauthenticated, "Unauthorized"));
                    }
                    CoreSettings settings = _settingsFactory.CreateCore(_settings.Value);
                    IEventId eventId = await GetInnerEventId(settings, domainId, requestStream.Current.EventId);
                    ITrace innerTrace = _traceFactory.Create(
                        domainId,
                        requestStream.Current.CreateTimestamp != null ? requestStream.Current.CreateTimestamp.ToDateTime() : default(DateTime?),
                        requestStream.Current.EventCode,
                        eventId);
                    innerTrace.Category = requestStream.Current.Category;
                    innerTrace.Data = GetData(requestStream.Current.Data);
                    innerTrace.Level = requestStream.Current.Level;
                    innerTrace.Message = requestStream.Current.Message;
                    await _traceSaver.Create(settings, innerTrace);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw new RpcException(new Status(StatusCode.Internal, "Internal System Error"), ex.Message);
                }
            }
            return new Empty();
        }

        private object GetData(Google.Protobuf.Collections.MapField<string, string> map)
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
