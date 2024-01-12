using BrassLoon.Interface.Address.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Address
{
    public class EmailAddressService : IEmailAddressService
    {
        private static readonly AsyncPolicy _cache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromSeconds(150)));

        public Task<EmailAddress> Get(ISettings settings, Guid domainId, Guid id)
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

        private static async Task<EmailAddress> InnerGet(ISettings settings, Guid domainId, Guid id)
        {
            Protos.GetEmailAddressRequest request = new Protos.GetEmailAddressRequest
            {
                DomainId = domainId.ToString("D"),
                EmailAddressId = id.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.EmailAddressService.EmailAddressServiceClient service = new Protos.EmailAddressService.EmailAddressServiceClient(channel);
                Protos.EmailAddress response = await service.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return EmailAddress.Create(response);
            }
        }

        public async Task<EmailAddress> Save(ISettings settings, EmailAddress emailAddress)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (emailAddress == null || (emailAddress.DomainId ?? Guid.Empty) == Guid.Empty)
                throw new ArgumentException("Address missing domain id value");
            AsyncPolicy retry = Policy.Handle<Exception>()
                .RetryAsync(2);
            return await retry.ExecuteAsync(async () =>
            {
                using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
                {
                    Protos.EmailAddressService.EmailAddressServiceClient service = new Protos.EmailAddressService.EmailAddressServiceClient(channel);
                    Protos.EmailAddress response = await service.SaveAsync(emailAddress.ToProto(), await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                    return EmailAddress.Create(response);
                }
            });
        }
    }
}
