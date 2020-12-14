using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
            return await _restUtil.Post<string>(_service, builder.Uri, clientCredential);
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
