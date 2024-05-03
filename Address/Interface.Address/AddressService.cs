using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;

namespace BrassLoon.Interface.Address
{
    public class AddressService : IAddressService
    {
        private static readonly AsyncPolicy _cache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromSeconds(150)));

        public Task<Models.Address> Get(ISettings settings, Guid domainId, Guid addressId)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (addressId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(addressId));
            AsyncPolicy retry = Policy.Handle<Exception>()
                .RetryAsync(2);
            return _cache.ExecuteAsync(
                context => retry.ExecuteAsync(() => InnerGet(settings, domainId, addressId)),
                new Context($"{domainId:N} {addressId:N}"));
        }

        private static async Task<Models.Address> InnerGet(ISettings settings, Guid domainId, Guid addressId)
        {
            Protos.GetAddressRequest request = new Protos.GetAddressRequest
            {
                DomainId = domainId.ToString("D"),
                AddressId = addressId.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.AddressService.AddressServiceClient service = new Protos.AddressService.AddressServiceClient(channel);
                Protos.Address response = await service.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Models.Address.Create(response);
            }
        }

        public async Task<Models.Address> Save(ISettings settings, Models.Address address)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (address == null || (address.DomainId ?? Guid.Empty) == Guid.Empty)
                throw new ArgumentException("Address missing domain id value");
            AsyncPolicy retry = Policy.Handle<Exception>()
                .RetryAsync(2);
            return await retry.ExecuteAsync(async () =>
            {
                using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
                {
                    Protos.AddressService.AddressServiceClient service = new Protos.AddressService.AddressServiceClient(channel);
                    Protos.Address response = await service.SaveAsync(address.ToProto(), await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                    return Models.Address.Create(response);
                }
            });
        }
    }
}
