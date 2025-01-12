using BrassLoon.Authorization.Data;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class RoleFactory : IRoleFactory
    {
        private static CachePolicy _domainCache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(6));
        private readonly IRoleDataFactory _dataFactory;
        private readonly IRoleDataSaver _dataSaver;

        public RoleFactory(IRoleDataFactory dataFactory, IRoleDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private Role Create(RoleData data) => new Role(data, _dataSaver);

        public IRole Create(Guid domainId, string policyName)
        {
            policyName = (policyName ?? string.Empty).Trim();
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (string.IsNullOrEmpty(policyName))
                throw new ArgumentNullException(nameof(policyName));
            return Create(
                new RoleData
                {
                    DomainId = domainId,
                    PolicyName = policyName
                });
        }

        public async Task<IRole> Get(ISettings settings, Guid domainId, Guid id)
        {
            Role role = null;
            RoleData data = await _dataFactory.Get(new CommonCore.DataSettings(settings), id);
            if (data != null && data.DomainId.Equals(domainId))
                role = Create(data);
            return role;
        }

        public Task<IEnumerable<IRole>> GetByDomainId(ISettings settings, Guid domainId)
        {
            return _domainCache.Execute(
                async context =>
                {
                    return (await _dataFactory.GetByDomainId(new CommonCore.DataSettings(settings), domainId))
                    .Select<RoleData, IRole>(Create);
                },
                new Context(domainId.ToString("D")));
        }

        public static void ClearCache() => _domainCache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(6));
    }
}
