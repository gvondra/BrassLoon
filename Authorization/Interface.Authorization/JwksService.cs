using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using Polly.Retry;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public class JwksService : IJwksService
    {
        private static readonly AsyncPolicy _cache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromSeconds(45));

        public Task<string> GetJwks(ISettings settings, Guid domainId) => GetJwks(new Uri(settings.BaseAddress), domainId);

        public async Task<string> GetJwks(Uri address, Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            return await _cache.ExecuteAsync(async context =>
            {
                return await RetryPolicy().ExecuteAsync(async () =>
                {
                    using (GrpcChannel channel = GrpcChannel.ForAddress(address))
                    {
                        Protos.JwksService.JwksServiceClient client = new Protos.JwksService.JwksServiceClient(channel);
                        Protos.GetJwksResponse response = await client.GetAsync(new Protos.GetByDomainRequest { DomainId = domainId.ToString("D") });
                        return response.Token;
                    }
                });
            },
            new Context());
        }

        private static AsyncRetryPolicy RetryPolicy()
        {
            return Policy.Handle<Exception>()
                .WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(750) });
        }
    }
}
