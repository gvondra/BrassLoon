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
    public class SigningKeyService : ISigningKeyService
    {
        private readonly IService _service;
        private readonly RestUtil _restUtil;

        public SigningKeyService(IService service, RestUtil restUtil)
        {
            _service = service;
            _restUtil = restUtil;
        }

        public Task<SigningKey> Create(ISettings settings, Guid domainId, SigningKey signingKey)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "SigningKey", domainId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Post, signingKey)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<SigningKey>(_service, request);
        }

        public Task<SigningKey> Create(ISettings settings, SigningKey signingKey)
        {
            if (!signingKey.DomainId.HasValue || signingKey.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(signingKey.DomainId));
            return Create(settings, signingKey.DomainId.Value, signingKey);
        }

        public Task<List<SigningKey>> GetByDomain(ISettings settings, Guid domainId)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "SigningKey", domainId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Get)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<SigningKey>>(_service, request);
        }

        public Task<SigningKey> Update(ISettings settings, Guid domainId, Guid signingKeyId, SigningKey signingKey)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "SigningKey", domainId.ToString("D"), signingKeyId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Put, signingKey)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<SigningKey>(_service, request);
        }

        public Task<SigningKey> Update(ISettings settings, SigningKey signingKey)
        {
            if (!signingKey.DomainId.HasValue || signingKey.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(signingKey.DomainId));
            if (!signingKey.SigningKeyId.HasValue || signingKey.SigningKeyId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(signingKey.SigningKeyId));
            return Update(settings, signingKey.DomainId.Value, signingKey.SigningKeyId.Value, signingKey);
        }
    }
}
