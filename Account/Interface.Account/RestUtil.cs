using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public sealed class RestUtil
    {
        private static Policy _cache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromMinutes(5)));

        public async Task<RestClient> CreateClient(ISettings settings)
        {
            string token = await settings.GetToken();
            HashAlgorithm hashAlgorithm = SHA256.Create();
            string hash = Convert.ToBase64String(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(token)));
            return _cache.Execute<RestClient>(
                context =>
                {
                    RestClient result = new RestClient(settings.BaseAddress);
                    result.UseJson()
                        .UseSerializer(() => new JsonNetSerializer(new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() }))
                        ;
                    result.Authenticator = new JwtAuthenticator(token);
                    return result;
                },
                new Context(string.Format("{0}::{1}", hash, settings.BaseAddress))
                );
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
                throw new ApplicationException($"Error {(int)restResponse.StatusCode} {restResponse.StatusDescription}: {restResponse.ErrorMessage}", restResponse.ErrorException);
            else if (!restResponse.IsSuccessful)
                throw new ApplicationException($"Error {(int)restResponse.StatusCode} {restResponse.StatusDescription}");
            return restResponse.Data;
        }
    }
}
