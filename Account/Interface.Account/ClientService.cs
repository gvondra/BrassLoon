using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class ClientService : IClientService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public ClientService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<Client> Create(ISettings settings, ClientCredentialRequest client)
        {
            if (string.IsNullOrEmpty(client?.Name))
                throw new ArgumentException($"Missing {nameof(Models.ClientCredentialRequest.Name)} value");
            if (string.IsNullOrEmpty(client?.Secret))
                throw new ArgumentException($"Missing {nameof(Models.ClientCredentialRequest.Secret)} value");
            if (!client.AccountId.HasValue || client.AccountId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"Missing or invalid {nameof(Models.ClientCredentialRequest.AccountId)} value");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, client)
                .AddPath("Client")
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Models.Client>(_service, request);
        }

        public Task<string> CreateSecret(ISettings settings)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("ClientSecret")
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<string>(_service, request);
        }

        public Task<Client> Get(ISettings settings, Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentException($"Missing or invalid {nameof(id)} value");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Client/{id}")
                .AddPathParameter("id", id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Client>(_service, request);
        }

        public Task<List<Client>> GetByAccountId(ISettings settings, Guid accountId)
        {
            if (accountId.Equals(Guid.Empty))
                throw new ArgumentException($"Missing or invalid {nameof(accountId)} value");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Account/{id}/Client")
                .AddPathParameter("id", accountId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<Client>>(_service, request);
        }

        public Task<Client> Update(ISettings settings, ClientCredentialRequest client)
        {
            if (string.IsNullOrEmpty(client?.Name))
                throw new ArgumentException($"Missing {nameof(Models.ClientCredentialRequest.Name)} value");
            if (!client.ClientId.HasValue || client.ClientId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"Missing or invalid {nameof(Models.ClientCredentialRequest.ClientId)} value");
            if (!client.AccountId.HasValue || client.AccountId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"Missing or invalid {nameof(Models.ClientCredentialRequest.AccountId)} value");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, client)
                .AddPath("Client/{id}")
                .AddPathParameter("id", client.ClientId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Models.Client>(_service, request);
        }
    }
}
