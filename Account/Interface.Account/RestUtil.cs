using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public sealed class RestUtil
    {
        internal RestUtil() { }

        public async Task<T> Send<T>(IService service, IRequest request)
        {
            IResponse<T> response = await service.Send<T>(request);
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Error {(int)response.StatusCode} {response.StatusCode}");
            return response.Value;
        }

        public async Task<T> Post<T>(IService service, Uri uri, object body)
        {
            IResponse<T> response = await service.Post<T>(uri, body);
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Error {(int)response.StatusCode} {response.StatusCode}");
            return response.Value;
        }
    }
}
