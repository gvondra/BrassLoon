using BrassLoon.Interface.Authorization.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public class ClientService : IClientService
    {
        public async Task<Client> Create(ISettings settings, Guid domainId, Client client)
        {
            Protos.Client request = Map(client);
            request.DomainId = domainId.ToString("D");
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.ClientService.ClientServiceClient clientService = new Protos.ClientService.ClientServiceClient(channel);
                Protos.Client response = await clientService.CreateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Map(response);
            }
        }

        public Task<Client> Create(ISettings settings, Client client)
        {
            if (!client.DomainId.HasValue || client.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"Missing {nameof(client.DomainId)} value");
            return Create(settings, client.DomainId.Value, client);
        }

        public async Task<Client> Get(ISettings settings, Guid domainId, Guid clientId)
        {
            Protos.GetClientRequest request = new Protos.GetClientRequest
            {
                DomainId = domainId.ToString("D"),
                ClientId = clientId.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.ClientService.ClientServiceClient clientService = new Protos.ClientService.ClientServiceClient(channel);
                Protos.Client response = await clientService.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Map(response);
            }
        }

        public async Task<List<Client>> GetByDomain(ISettings settings, Guid domainId)
        {
            Protos.GetByDomainRequest request = new Protos.GetByDomainRequest
            {
                DomainId = domainId.ToString("D"),
            };
            List<Client> results = new List<Client>();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.ClientService.ClientServiceClient clientService = new Protos.ClientService.ClientServiceClient(channel);
                AsyncServerStreamingCall<Protos.Client> stream = clientService.GetByDomain(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await stream.ResponseStream.MoveNext())
                {
                    results.Add(
                        Map(stream.ResponseStream.Current));
                }
            }
            return results;
        }

        public async Task<string> GetClientCredentialSecret(ISettings settings)
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.ClientService.ClientServiceClient clientService = new Protos.ClientService.ClientServiceClient(channel);
                Protos.ClientCredentialSecret response = await clientService.GetClientCredentialSecretAsync(new Empty(), await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return response.Secret;
            }
        }

        public async Task<Client> Update(ISettings settings, Guid domainId, Guid clientId, Client client)
        {
            Protos.Client request = Map(client);
            request.DomainId = domainId.ToString("D");
            request.ClientId = clientId.ToString("D");
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.ClientService.ClientServiceClient clientService = new Protos.ClientService.ClientServiceClient(channel);
                Protos.Client response = await clientService.UpdateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Map(response);
            }
        }

        public Task<Client> Update(ISettings settings, Client client)
        {
            if (!client.DomainId.HasValue || client.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"Missing {nameof(client.DomainId)} value");
            if (!client.ClientId.HasValue || client.ClientId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"Missing {nameof(client.ClientId)} value");
            return Update(settings, client.DomainId.Value, client.ClientId.Value, client);
        }

        private static Client Map(Protos.Client client)
        {
            Client result = new Client
            {
                ClientId = !string.IsNullOrEmpty(client.ClientId) ? Guid.Parse(client.ClientId) : default(Guid?),
                CreateTimestamp = client.CreateTimestamp?.ToDateTime(),
                DomainId = !string.IsNullOrEmpty(client.DomainId) ? Guid.Parse(client.DomainId) : default(Guid?),
                IsActive = client.IsActive,
                Name = client.Name,
                Roles = new List<AppliedRole>(),
                Secret = client.Secret,
                UpdateTimestamp = client.UpdateTimestamp?.ToDateTime(),
                UserEmailAddress = client.UserEmailAddress,
                UserName = client.UserName
            };
            if (client.Roles != null)
            {
                foreach (Protos.AppliedRole r in client.Roles)
                {
                    result.Roles.Add(Map(r));
                }
            }
            return result;
        }

        private static Protos.Client Map(Client client)
        {
            Protos.Client result = new Protos.Client
            {
                ClientId = client.ClientId.HasValue ? client.ClientId.Value.ToString("D") : string.Empty,
                CreateTimestamp = client.CreateTimestamp.HasValue ? Timestamp.FromDateTime(client.CreateTimestamp.Value) : null,
                DomainId = client.DomainId.HasValue ? client.DomainId.Value.ToString("D") : string.Empty,
                IsActive = client.IsActive,
                Name = client.Name,
                Secret = client.Secret,
                UpdateTimestamp = client.UpdateTimestamp.HasValue ? Timestamp.FromDateTime(client.UpdateTimestamp.Value) : null,
                UserEmailAddress = client.UserEmailAddress,
                UserName = client.UserName
            };
            if (client.Roles != null)
            {
                foreach (AppliedRole r in client.Roles)
                {
                    result.Roles.Add(Map(r));
                }
            }
            return result;
        }

        private static AppliedRole Map(Protos.AppliedRole appliedRole)
        {
            return new AppliedRole
            {
                Name = appliedRole.Name,
                PolicyName = appliedRole.PolicyName
            };
        }

        private static Protos.AppliedRole Map(AppliedRole appliedRole)
        {
            return new Protos.AppliedRole
            {
                Name = appliedRole.Name,
                PolicyName = appliedRole.PolicyName
            };
        }
    }
}
