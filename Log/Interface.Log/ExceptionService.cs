using LogModels = BrassLoon.Interface.Log.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace BrassLoon.Interface.Log
{
    public class ExceptionService : IExceptionService
    {
        private readonly RestUtil _restUtil;

        public ExceptionService(RestUtil restUtil)
        {
            _restUtil = restUtil;
        }

        public async Task<LogModels.Exception> Create(ISettings settings, LogModels.Exception exception)
        {
            RestRequest request = new RestRequest("Exception", Method.POST, DataFormat.Json);
            request.AddJsonBody(exception);
            return await _restUtil.Execute<LogModels.Exception>(settings, request);
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
