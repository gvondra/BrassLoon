using BrassLoon.RestClient;
using Polly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using LogModels = BrassLoon.Interface.Log.Models;

namespace BrassLoon.Interface.Log
{
    public class ExceptionService : IExceptionService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public ExceptionService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<LogModels.Exception> Create(ISettings settings, LogModels.Exception exception)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, exception)
            .AddPath("Exception")
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse<LogModels.Exception> response = await Policy
                .HandleResult<IResponse<LogModels.Exception>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2.0 + Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => _service.Send<LogModels.Exception>(request))
                ;
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<LogModels.Exception> Create(ISettings settings, Guid domainId, Exception exception) => Create(settings, domainId, null, exception);

        public Task<LogModels.Exception> Create(ISettings settings, Guid domainId, DateTime? createTimestamp, Exception exception)
        {
            return Create(
                settings,
                domainId,
                exception,
                createTimestamp: createTimestamp);
        }

        public Task<LogModels.Exception> Create(
            ISettings settings,
            Guid domainId,
            Exception exception,
            DateTime? createTimestamp = null,
            string category = null,
            string level = null,
            LogModels.EventId? eventId = null)
        {
            return Create(
                settings,
                CreateException(domainId, exception, createTimestamp, category, level, eventId));
        }

        public Task<LogModels.Exception> Get(ISettings settings, Guid domainId, long id)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
            .AddPath("Exception")
            .AddPath(domainId.ToString("N"))
            .AddPath(id.ToString(CultureInfo.InvariantCulture))
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            return _restUtil.Send<LogModels.Exception>(_service, request);
        }

        public Task<List<LogModels.Exception>> Search(ISettings settings, Guid domainId, DateTime maxTimestamp)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
            .AddPath("Exception")
            .AddPath(domainId.ToString("N"))
            .AddQueryParameter("maxTimestamp", maxTimestamp.ToString("o"))
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            return _restUtil.Send<List<LogModels.Exception>>(_service, request);
        }

        private static LogModels.Exception CreateException(
            Guid domainId,
            Exception exception,
            DateTime? createTimestamp,
            string category,
            string level,
            LogModels.EventId? eventId)
        {
            LogModels.Exception innerException = null;
            if (exception.InnerException != null)
                innerException = CreateException(domainId, exception.InnerException, createTimestamp, category, level, eventId);
            return new LogModels.Exception
            {
                AppDomain = AppDomain.CurrentDomain.FriendlyName,
                DomainId = domainId,
                Message = exception.Message,
                Source = exception.Source,
                StackTrace = exception.StackTrace,
                TargetSite = exception.TargetSite?.ToString() ?? string.Empty,
                TypeName = exception.GetType().FullName,
                InnerException = innerException,
                Data = GetData(exception.Data),
                CreateTimestamp = createTimestamp,
                Category = category,
                Level = level,
                EventId = eventId
            };
        }

        private static Dictionary<string, string> GetData(IDictionary data)
        {
            Dictionary<string, string> result = null;
            if (data != null)
            {
                result = new Dictionary<string, string>();
                IDictionaryEnumerator enumerator = data.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string key = (enumerator.Key ?? string.Empty).ToString();
                    string value = (enumerator.Value ?? string.Empty).ToString();
                    result.Add(key, value);
                }
            }
            return result;
        }
    }
}
