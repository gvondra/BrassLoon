using BrassLoon.Interface.Authorization.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public class TokenService : ITokenService
    {
        private readonly IService _service;
        private readonly RestUtil _restUtil;

        public TokenService(IService service, RestUtil restUtil)
        {
            _service = service;
            _restUtil = restUtil;
        }

        public Task<string> Create(ISettings settings, Guid domainId)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Token", domainId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Post);
            return _restUtil.Send<string>(_service, request);
        }

        public Task<string> CreateClientCredential(ISettings settings, Guid domainId, ClientCredential clientCredential)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Token/ClientCredential", domainId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Post, clientCredential);
            return _restUtil.Send<string>(_service, request);
        }
    }
}
