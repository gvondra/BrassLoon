using BrassLoon.Interface.Authorization.Models;
using Google.Protobuf.WellKnownTypes;
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
            Protos.Role request = Map(role);
            request.DomainId = domainId.ToString("D");
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.RoleService.RoleServiceClient roleService = new Protos.RoleService.RoleServiceClient(channel);
                Protos.Role response = await roleService.CreateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Map(response);
            }
        }

        public Task<Role> Create(ISettings settings, Role role)
        {
            if (!role.DomainId.HasValue || role.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(role.DomainId));
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
                        Map(stream.ResponseStream.Current));
                }
            }
            return results;
        }

        public async Task<Role> Update(ISettings settings, Guid domainId, Guid roleId, Role role)
        {
            Protos.Role request = Map(role);
            request.RoleId = roleId.ToString("D");
            request.DomainId = domainId.ToString("D");
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.RoleService.RoleServiceClient roleService = new Protos.RoleService.RoleServiceClient(channel);
                Protos.Role response = await roleService.UpdateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Map(response);
            }
        }

        public Task<Role> Update(ISettings settings, Role role)
        {
            if (!role.DomainId.HasValue || role.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(role.DomainId));
            if (!role.RoleId.HasValue || role.RoleId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(role.RoleId));
            return Update(settings, role.DomainId.Value, role.RoleId.Value, role);
        }

        private static Role Map(Protos.Role role)
        {
            return new Role
            {
                Comment = role.Comment,
                CreateTimestamp = role.CreateTimestamp?.ToDateTime(),
                DomainId = !string.IsNullOrEmpty(role.DomainId) ? Guid.Parse(role.DomainId) : default(Guid?),
                IsActive = role.IsActive,
                Name = role.Name,
                PolicyName = role.PolicyName,
                RoleId = !string.IsNullOrEmpty(role.RoleId) ? Guid.Parse(role.RoleId) : default(Guid?),
                UpdateTimestamp = role.UpdateTimestamp?.ToDateTime()
            };
        }

        private static Protos.Role Map(Role innerRole)
        {
            return new Protos.Role
            {
                Comment = innerRole.Comment,
                CreateTimestamp = innerRole.CreateTimestamp.HasValue ? Timestamp.FromDateTime(innerRole.CreateTimestamp.Value) : null,
                DomainId = innerRole.DomainId?.ToString("D"),
                IsActive = innerRole.IsActive,
                Name = innerRole.Name,
                PolicyName = innerRole.PolicyName,
                RoleId = innerRole.RoleId?.ToString("D"),
                UpdateTimestamp = innerRole.UpdateTimestamp.HasValue ? Timestamp.FromDateTime(innerRole.UpdateTimestamp.Value) : null
            };
        }
    }
}
