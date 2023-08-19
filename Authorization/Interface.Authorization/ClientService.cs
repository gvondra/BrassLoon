using BrassLoon.Interface.Authorization.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public class ClientService : IClientService
    {
        private readonly IService _service;
        private readonly RestUtil _restUtil;

        public ClientService(IService service, RestUtil restUtil)
        {
            _service = service;
            _restUtil = restUtil;
        }

        public Task<Client> Create(ISettings settings, Guid domainId, Client client)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Client", domainId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Post, client)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Client>(_service, request);
        }

        public Task<Client> Create(ISettings settings, Client client)
        {
            if (!client.DomainId.HasValue || client.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(client.DomainId));
            return Create(settings, client.DomainId.Value, client);
        }

        public Task<Client> Get(ISettings settings, Guid domainId, Guid clientId)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Client", domainId.ToString("D"), clientId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Get)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Client>(_service, request);
        }

        public Task<List<Client>> GetByDomain(ISettings settings, Guid domainId)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Client", domainId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Get)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<Client>>(_service, request);
        }

        public Task<string> GetClientCredentialSecret(ISettings settings)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "ClientCredentialSecret");
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Get)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<string>(_service, request);
        }

        public Task<Client> Update(ISettings settings, Guid domainId, Guid clientId, Client client)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Client", domainId.ToString("D"), clientId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Put, client)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Client>(_service, request);
        }

        public Task<Client> Update(ISettings settings, Client client)
        {
            if (!client.DomainId.HasValue || client.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(client.DomainId));
            if (!client.ClientId.HasValue || client.ClientId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(client.ClientId));
            return Update(settings, client.DomainId.Value, client.ClientId.Value, client);
        }
    }
}
