using BrassLoon.Interface.Account.Models;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class TokenService : ITokenService
    {
        private readonly RestUtil _restUtil;

        public TokenService(RestUtil restUtil)
        {
            _restUtil = restUtil;
        }

        public async Task<string> CreateClientCredentialToken(ISettings settings, ClientCredential clientCredential)
        {
            RestClient client = new RestClient(settings.BaseAddress);
            client.UseJson()
                .UseSerializer(() => new JsonNetSerializer(new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() }))
                ;            
            RestRequest request = new RestRequest("Token/ClientCredential", Method.POST, DataFormat.Json);            
            request.AddJsonBody(clientCredential);
            IRestResponse restResponse = await client.ExecuteAsync(request);
            if (restResponse.ErrorException != null)
                throw new ApplicationException($"Error {(int)restResponse.StatusCode} {restResponse.StatusDescription}: {restResponse.ErrorMessage}", restResponse.ErrorException);
            else if (!restResponse.IsSuccessful)
                throw new ApplicationException($"Error {(int)restResponse.StatusCode} {restResponse.StatusDescription}");
            return restResponse.Content;
        }

        public Task<string> CreateClientCredentialToken(ISettings settings, Guid clientId, string secret)
        {
            return CreateClientCredentialToken(
                settings,
                new ClientCredential()
                {
                    ClientId = clientId,
                    Secret = secret
                }
                );
        }
    }
}
