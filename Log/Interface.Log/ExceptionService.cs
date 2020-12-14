using LogModels = BrassLoon.Interface.Log.Models;
using BrassLoon.RestClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            return await _restUtil.Send<LogModels.Exception>(_service, request);
        }

        public Task<LogModels.Exception> Create(ISettings settings, Guid domainId, System.Exception exception)
        {
            return Create(
                settings,
                CreateException(domainId, exception)
                );
        }

        private LogModels.Exception CreateException(Guid domainId, System.Exception exception)
        {
            LogModels.Exception innerException = null;
            if (exception.InnerException != null)
                innerException = CreateException(domainId, exception.InnerException);
            return new LogModels.Exception
            {
                AppDomain = AppDomain.CurrentDomain.FriendlyName,
                DomainId = domainId,
                Message = exception.Message,
                Source = exception.Source,
                StackTrace = exception.StackTrace, 
                TargetSite = exception.TargetSite.Name,
                TypeName = exception.GetType().FullName,
                InnerException = innerException,
                Data = GetData(exception.Data)
            };
        }

        private object GetData(IDictionary data)
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
