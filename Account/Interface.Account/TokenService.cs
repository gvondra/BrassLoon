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
        private readonly IService _service;

        public TokenService(IService service)
        {
            _service = service;
        }

        public async Task<string> CreateClientCredentialToken(ISettings settings, ClientCredential clientCredential)
        {
            UriBuilder builder = new UriBuilder(settings.BaseAddress);
            builder.Path = string.Concat(builder.Path.Trim('/'), "/", "Token/ClientCredential").Trim('/');
            return (await _service.Post<string>(builder.Uri, clientCredential)).Value;
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
