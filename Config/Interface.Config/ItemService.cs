using BrassLoon.Interface.Config.Models;
using BrassLoon.RestClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Config
{
    public class ItemService : IItemService
    {
        private static Policy _cache = CreateCache();
        private readonly IService _service;
        private readonly RestUtil _restUtil;

        public ItemService(IService service, RestUtil restUtil)
        {
            _service = service;
            _restUtil = restUtil;
        }

        private static Policy CreateCache() => Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromSeconds(45));

        public async Task Delete(ISettings settings, Guid domainId, string code)
        {
            try
            {
                IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Delete)
                .AddPath("Item/{domainId}/{code}")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddPathParameter("code", code)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
                IResponse response = await _service.Send(request);
                _restUtil.CheckSuccess(response);
            }
            finally
            {
                _cache = CreateCache();
            }
        }

        public async Task<Item> GetByCode(ISettings settings, Guid domainId, string code)
        {
            return await _cache.Execute(async (context) => await InnerGetByCode(settings, domainId, code),
                new Context($"Get{domainId:N}|{code}"));
        }

        private async Task<Item> InnerGetByCode(ISettings settings, Guid domainId, string code)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
            .AddPath("Item/{domainId}/{code}")
            .AddPathParameter("domainId", domainId.ToString("N"))
            .AddPathParameter("code", code)
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse<Item> response = await _service.Send<Item>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<List<string>> GetCodes(ISettings settings, Guid domainId)
        {
            return await _cache.Execute(async (context) => await InnerGetCodes(settings, domainId),
            new Context($"Codes{domainId:N}"));
        }

        private async Task<List<string>> InnerGetCodes(ISettings settings, Guid domainId)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
            .AddPath("ItemCode/{domainId}")
            .AddPathParameter("domainId", domainId.ToString("N"))
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse<List<string>> response = await _service.Send<List<string>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<object> GetDataByCode(ISettings settings, Guid domainId, string code)
        {
            return await _cache.Execute(async (context) => await InnerGetDataByCode(settings, domainId, code),
                new Context($"GetData{domainId:N}|{code}"));
        }

        private async Task<object> InnerGetDataByCode(ISettings settings, Guid domainId, string code)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
            .AddPath("Item/{domainId}/{code}/Data")
            .AddPathParameter("domainId", domainId.ToString("N"))
            .AddPathParameter("code", code)
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse<object> response = await _service.Send<object>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<List<ItemHistory>> GetHistoryByCode(ISettings settings, Guid domainId, string code)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
            .AddPath("Item/{domainId}/{code}/History")
            .AddPathParameter("domainId", domainId.ToString("N"))
            .AddPathParameter("code", code)
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse<List<ItemHistory>> response = await _service.Send<List<ItemHistory>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<Item> Save(ISettings settings, Guid domainId, string code, object data)
        {
            try
            {
                IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, data)
                .AddPath("Item/{domainId}/{code}/Data")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddPathParameter("code", code)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
                IResponse<Item> response = await _service.Send<Item>(request);
                _restUtil.CheckSuccess(response);
                return response.Value;
            }
            finally
            {
                _cache = CreateCache();
            }
        }
    }
}
