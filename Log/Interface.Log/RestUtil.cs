using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Log
{
    public sealed class RestUtil
    {
        public async Task<RestClient> CreateClient(ISettings settings)
        {
            RestClient client = new RestClient(settings.BaseAddress);
            client.UseJson()
                .UseSerializer(() => new JsonNetSerializer(new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() }))
                ;
            client.Authenticator = new JwtAuthenticator(await settings.GetToken());
            return client;
        }

        public async Task<T> Execute<T>(ISettings settings, RestRequest request)
        {
            return await Execute<T>(
                await CreateClient(settings),
                request
                );
        }

        public async Task<T> Execute<T>(RestClient client, RestRequest request)
        {
            IRestResponse<T> restResponse = await client.ExecuteAsync<T>(request);
            if (restResponse.ErrorException != null)
                throw new ApplicationException($"Error {restResponse.StatusCode.ToString()} {restResponse.StatusDescription}: {restResponse.ErrorMessage}", restResponse.ErrorException);
            else if (!restResponse.IsSuccessful)
                throw new ApplicationException($"Error {restResponse.StatusCode.ToString()} {restResponse.StatusDescription}");
            return restResponse.Data;
        }
    }
}
