using BrassLoon.Interface.Account.Models;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class DomainService : IDomainService
    {
        public async Task<Domain> Get(ISettings settings, Guid id)
        {
            RestClient client = new RestClient(settings.BaseAddress);
            client.UseJson()
                .UseSerializer(() => new JsonNetSerializer(new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() }))
                ;
            client.Authenticator = new JwtAuthenticator(await settings.GetToken());
            RestRequest request = new RestRequest("Domain/{id}", Method.GET, DataFormat.Json);
            request.AddParameter("id", id.ToString(), ParameterType.UrlSegment);
            IRestResponse<Domain> restResponse = await client.ExecuteAsync<Domain>(request);
            if (restResponse.ErrorException != null)
                throw new ApplicationException($"Error {restResponse.StatusCode.ToString()} {restResponse.StatusDescription}: {restResponse.ErrorMessage}", restResponse.ErrorException);
            else if (!restResponse.IsSuccessful)
                throw new ApplicationException($"Error {restResponse.StatusCode.ToString()} {restResponse.StatusDescription}");
            return restResponse.Data;
        }
    }
}
