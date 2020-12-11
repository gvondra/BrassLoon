using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Data.Models;
using BrassLoon.Log.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class Exception : IException
    {
        private readonly ExceptionData _data;
        private readonly IExceptionDataSaver _dataSaver;
        private readonly IExceptionFactory _exceptionFactory;

        public Exception(ExceptionData data,
            IExceptionDataSaver dataSaver,
            IExceptionFactory exceptionFactory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _exceptionFactory = exceptionFactory;
        }

        public long ExceptionId => _data.ExceptionId;

        public Guid DomainId => _data.DomainId;

        public string Message { get => _data.Message; set => _data.Message = value; }
        public string TypeName { get => _data.TypeName; set => _data.TypeName = value; }
        public string Source { get => _data.Source; set => _data.Source = value; }
        public string AppDomain { get => _data.AppDomain; set => _data.AppDomain = value; }
        public string TargetSite { get => _data.TargetSite; set => _data.TargetSite = value; }
        public string StackTrace { get => _data.StackTrace; set => _data.StackTrace = value; }
        
        public dynamic Data 
        {
            get
            {
                if (!string.IsNullOrEmpty(_data.Data))
                    return JsonConvert.DeserializeObject(_data.Data);
                else
                    return null;
            }
            set
            {
                if (value != null)
                    _data.Data = JsonConvert.SerializeObject(value);
                else
                    _data.Data = null;
            }
        }

        internal IException ParentException { get; set; }
        private long? ParentExceptionId { get => _data.ParentExceptionId; set => _data.ParentExceptionId = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public async Task Create(ITransactionHandler transactionHandler)
        {
            if (ParentException != null)
                ParentExceptionId = ParentException.ExceptionId;
            await _dataSaver.Create(transactionHandler, _data);
        }

        public async Task<IException> GetInnerException(ISettings settings)
        {
            return await _exceptionFactory.GetInnerException(settings, ExceptionId);
        }
    }
}
