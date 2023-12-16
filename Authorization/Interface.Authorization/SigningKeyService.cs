using BrassLoon.Interface.Authorization.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public class SigningKeyService : ISigningKeyService
    {
        public async Task<SigningKey> Create(ISettings settings, Guid domainId, SigningKey signingKey)
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.SigningKeyService.SigningKeyServiceClient client = new Protos.SigningKeyService.SigningKeyServiceClient(channel);
                Protos.SigningKey request = Map(signingKey);
                request.DomainId = domainId.ToString("D");
                Protos.SigningKey result = await client.CreateAsync(
                    request,
                    headers: await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Map(result);
            }
        }

        public Task<SigningKey> Create(ISettings settings, SigningKey signingKey)
        {
            if (!signingKey.DomainId.HasValue || signingKey.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(signingKey.DomainId));
            return Create(settings, signingKey.DomainId.Value, signingKey);
        }

        public async Task<List<SigningKey>> GetByDomain(ISettings settings, Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.SigningKeyService.SigningKeyServiceClient client = new Protos.SigningKeyService.SigningKeyServiceClient(channel);
                AsyncServerStreamingCall<Protos.SigningKey> stream = client.GetByDomain(
                    new Protos.GetByDomainRequest { DomainId = domainId.ToString("D") },
                    await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                List<SigningKey> result = new List<SigningKey>();
                while (await stream.ResponseStream.MoveNext())
                {
                    result.Add(
                        Map(stream.ResponseStream.Current));
                }
                return result;
            }
        }

        public async Task<SigningKey> Update(ISettings settings, Guid domainId, Guid signingKeyId, SigningKey signingKey)
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.SigningKeyService.SigningKeyServiceClient client = new Protos.SigningKeyService.SigningKeyServiceClient(channel);
                Protos.SigningKey request = Map(signingKey);
                request.SigningKeyId = signingKeyId.ToString("D");
                request.DomainId = domainId.ToString("D");
                Protos.SigningKey result = await client.UpdateAsync(
                    request,
                    headers: await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Map(result);
            }
        }

        public Task<SigningKey> Update(ISettings settings, SigningKey signingKey)
        {
            if (!signingKey.DomainId.HasValue || signingKey.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(signingKey.DomainId));
            if (!signingKey.SigningKeyId.HasValue || signingKey.SigningKeyId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(signingKey.SigningKeyId));
            return Update(settings, signingKey.DomainId.Value, signingKey.SigningKeyId.Value, signingKey);
        }

        private static Protos.SigningKey Map(SigningKey signingKey)
        {
            return new Protos.SigningKey
            {
                CreateTimestamp = signingKey.CreateTimestamp.HasValue ? Timestamp.FromDateTime(signingKey.CreateTimestamp.Value) : default,
                DomainId = signingKey.DomainId.HasValue ? signingKey.DomainId.Value.ToString("D") : string.Empty,
                IsActive = signingKey.IsActive,
                SigningKeyId = signingKey.SigningKeyId.HasValue ? signingKey.SigningKeyId.Value.ToString("D") : string.Empty,
                UpdateTimestamp = signingKey.UpdateTimestamp.HasValue ? Timestamp.FromDateTime(signingKey.UpdateTimestamp.Value) : default
            };
        }

        private static SigningKey Map(Protos.SigningKey signingKey)
        {
            return new SigningKey
            {
                CreateTimestamp = signingKey.CreateTimestamp != default ? signingKey.CreateTimestamp.ToDateTime() : default,
                DomainId = !string.IsNullOrEmpty(signingKey.DomainId) ? Guid.Parse(signingKey.DomainId) : default(Guid?),
                IsActive = signingKey.IsActive,
                SigningKeyId = !string.IsNullOrEmpty(signingKey.SigningKeyId) ? Guid.Parse(signingKey.SigningKeyId) : default(Guid?),
                UpdateTimestamp = signingKey.UpdateTimestamp != default ? signingKey.UpdateTimestamp.ToDateTime() : default
            };
        }
    }
}
