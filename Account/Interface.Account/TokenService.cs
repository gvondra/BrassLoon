using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class TokenService : ITokenService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public TokenService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<string> CreateClientCredentialToken(ISettings settings, ClientCredential clientCredential)
        {
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = string.Concat(builder.Path.Trim('/'), "/", "Token/ClientCredential").Trim('/');
            IResponse<string> response = await Policy
                .HandleResult<IResponse<string>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => _service.Post<string>(builder.Uri, clientCredential))
                ;
            _restUtil.CheckSuccess(response);
            return response.Value;
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
