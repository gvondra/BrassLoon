using BrassLoon.Interface.Authorization.Models;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public class RoleService : IRoleService
    {
        public async Task<Role> Create(ISettings settings, Guid domainId, Role role)
        {
            Protos.Role request = role.ToProto();
            request.DomainId = domainId.ToString("D");
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.RoleService.RoleServiceClient roleService = new Protos.RoleService.RoleServiceClient(channel);
                Protos.Role response = await roleService.CreateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Role.Create(response);
            }
        }

        public Task<Role> Create(ISettings settings, Role role)
        {
            if (!role.DomainId.HasValue || role.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(role.DomainId)} property of Role is not set");
            return Create(settings, role.DomainId.Value, role);
        }

        public async Task<List<Role>> GetByDomainId(ISettings settings, Guid domainId)
        {
            Protos.GetByDomainRequest request = new Protos.GetByDomainRequest
            {
                DomainId = domainId.ToString("D"),
            };
            List<Role> results = new List<Role>();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.RoleService.RoleServiceClient roleService = new Protos.RoleService.RoleServiceClient(channel);
                AsyncServerStreamingCall<Protos.Role> stream = roleService.GetByDomainId(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await stream.ResponseStream.MoveNext())
                {
                    results.Add(
                        Role.Create(stream.ResponseStream.Current));
                }
            }
            return results;
        }

        public async Task<Role> Update(ISettings settings, Guid domainId, Guid roleId, Role role)
        {
            Protos.Role request = role.ToProto();
            request.RoleId = roleId.ToString("D");
            request.DomainId = domainId.ToString("D");
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.RoleService.RoleServiceClient roleService = new Protos.RoleService.RoleServiceClient(channel);
                Protos.Role response = await roleService.UpdateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Role.Create(response);
            }
        }

        public Task<Role> Update(ISettings settings, Role role)
        {
            if (!role.DomainId.HasValue || role.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(role.DomainId)} property of Role is not set");
            if (!role.RoleId.HasValue || role.RoleId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(role.RoleId)} property of Role is not set");
            return Update(settings, role.DomainId.Value, role.RoleId.Value, role);
        }
    }
}
