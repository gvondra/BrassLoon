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
    public class RoleService : IRoleService
    {
        private readonly IService _service;
        private readonly RestUtil _restUtil;

        public RoleService(IService service, RestUtil restUtil)
        {
            _service = service;
            _restUtil = restUtil;
        }

        public Task<Role> Create(ISettings settings, Guid domainId, Role role)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Role", domainId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Post, role)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Role>(_service, request);
        }

        public Task<Role> Create(ISettings settings, Role role)
        {
            if (!role.DomainId.HasValue || role.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(role.DomainId));
            return Create(settings, role.DomainId.Value, role);
        }

        public Task<List<Role>> GetByDomainId(ISettings settings, Guid domainId)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Role", domainId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Get)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<Role>>(_service, request);
        }

        public Task<Role> Update(ISettings settings, Guid domainId, Guid roleId, Role role)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Role", domainId.ToString("D"), roleId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Put, role)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Role>(_service, request);
        }

        public Task<Role> Update(ISettings settings, Role role)
        {
            if (!role.DomainId.HasValue || role.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(role.DomainId));
            if (!role.RoleId.HasValue || role.RoleId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(role.RoleId));
            return Update(settings, role.DomainId.Value, role.RoleId.Value, role);
        }
    }
}
