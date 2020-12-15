using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public sealed class RestUtil
    {
        internal RestUtil() { }

        public async Task<T> Send<T>(IService service, IRequest request)
        {
            IResponse<T> response = await service.Send<T>(request);
            CheckSuccess(response);
            return response.Value;
        }

        internal void CheckSuccess(IResponse response)
        {
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Error {(int)response.StatusCode} {response.StatusCode}");
        }
    }
}
