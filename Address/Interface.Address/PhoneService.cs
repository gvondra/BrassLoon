using System;
using System.Threading.Tasks;
using BrassLoon.Interface.Address.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;

namespace BrassLoon.Interface.Address
{
    public class PhoneService : IPhoneService
    {
        private static readonly AsyncPolicy _cache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromSeconds(150)));

        public Task<Phone> Get(ISettings settings, Guid domainId, Guid id)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            AsyncPolicy retry = Policy.Handle<Exception>()
                .RetryAsync(2);
            return _cache.ExecuteAsync(
                context => retry.ExecuteAsync(() => InnerGet(settings, domainId, id)),
                new Context($"{domainId:N} {id:N}"));
        }

        private static async Task<Phone> InnerGet(ISettings settings, Guid domainId, Guid id)
        {
            Protos.GetPhoneRequest request = new Protos.GetPhoneRequest
            {
                DomainId = domainId.ToString("D"),
                PhoneId = id.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.PhoneService.PhoneServiceClient service = new Protos.PhoneService.PhoneServiceClient(channel);
                Protos.Phone response = await service.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Phone.Create(response);
            }
        }

        public async Task<Phone> Save(ISettings settings, Phone phone)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (phone == null || (phone.DomainId ?? Guid.Empty) == Guid.Empty)
                throw new ArgumentException("Address missing domain id value");
            AsyncPolicy retry = Policy.Handle<Exception>()
                .RetryAsync(2);
            return await retry.ExecuteAsync(async () =>
            {
                using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
                {
                    Protos.PhoneService.PhoneServiceClient service = new Protos.PhoneService.PhoneServiceClient(channel);
                    Protos.Phone response = await service.SaveAsync(phone.ToProto(), await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                    return Phone.Create(response);
                }
            });
        }
    }
}
