﻿using BrassLoon.Interface.Config.Models;
using BrassLoon.RestClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Config
{
#pragma warning disable S2696 // Instance members should not write to "static" fields
    public class LookupService : ILookupService
    {
        private static Policy _cache = CreateCache();
        private readonly IService _service;
        private readonly RestUtil _restUtil;

        public LookupService(IService service, RestUtil restUtil)
        {
            _service = service;
            _restUtil = restUtil;
        }

        private static CachePolicy CreateCache() => Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromSeconds(45));

        public async Task Delete(ISettings settings, Guid domainId, string code)
        {
            try
            {
                IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Delete)
                .AddPath("Lookup/{domainId}/{code}")
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

        public async Task<Lookup> GetByCode(ISettings settings, Guid domainId, string code)
        {
            return await _cache.Execute(
                async (context) => await InnerGetByCode(settings, domainId, code),
                new Context($"Get{domainId:N}|{code}"));
        }

        private async Task<Lookup> InnerGetByCode(ISettings settings, Guid domainId, string code)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
            .AddPath("Lookup/{domainId}/{code}")
            .AddPathParameter("domainId", domainId.ToString("N"))
            .AddPathParameter("code", code)
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse<Lookup> response = await _service.Send<Lookup>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<List<string>> GetCodes(ISettings settings, Guid domainId)
        {
            return await _cache.Execute(
                async (context) => await InnerGetCodes(settings, domainId),
                new Context($"Codes{domainId:N}"));
        }

        private async Task<List<string>> InnerGetCodes(ISettings settings, Guid domainId)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
            .AddPath("LookupCode/{domainId}")
            .AddPathParameter("domainId", domainId.ToString("N"))
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse<List<string>> response = await _service.Send<List<string>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<Dictionary<string, string>> GetDataByCode(ISettings settings, Guid domainId, string code)
        {
            return await _cache.Execute(
                async (context) => await InnerGetDataByCode(settings, domainId, code),
                new Context($"GetData{domainId:N}|{code}"));
        }

        private async Task<Dictionary<string, string>> InnerGetDataByCode(ISettings settings, Guid domainId, string code)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
            .AddPath("Lookup/{domainId}/{code}/Data")
            .AddPathParameter("domainId", domainId.ToString("N"))
            .AddPathParameter("code", code)
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse<Dictionary<string, string>> response = await _service.Send<Dictionary<string, string>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<List<LookupHistory>> GetHistoryByCode(ISettings settings, Guid domainId, string code)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
            .AddPath("Lookup/{domainId}/{code}/History")
            .AddPathParameter("domainId", domainId.ToString("N"))
            .AddPathParameter("code", code)
            .AddJwtAuthorizationToken(settings.GetToken)
            ;
            IResponse<List<LookupHistory>> response = await _service.Send<List<LookupHistory>>(request);
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

#pragma warning disable IDE0078 // Use pattern matching
#pragma warning disable IDE0083 // Use pattern matching
        public Task<Lookup> Save(ISettings settings, Guid domainId, string code, object data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (!(data is Dictionary<string, string>))
                throw new ApplicationException("Parameter \"data\" must of type Dictionary<string, string>");
            return Save(settings, domainId, code, (Dictionary<string, string>)data);
        }
#pragma warning restore IDE0078 // Use pattern matching
#pragma warning restore IDE0083 // Use pattern matching

        public Task<Lookup> Save(ISettings settings, Guid domainId, string code, Dictionary<string, string> data)
        {
            try
            {
                if (domainId.Equals(Guid.Empty))
                    throw new ArgumentNullException(nameof(domainId));
                if (string.IsNullOrEmpty(code))
                    throw new ArgumentNullException(nameof(code));
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, data)
                .AddPath("Lookup/{domainId}/{code}/Data")
                .AddPathParameter("domainId", domainId.ToString("N"))
                .AddPathParameter("code", code)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
                return _restUtil.Send<Lookup>(_service, request);
            }
            finally
            {
                _cache = CreateCache();
            }
        }
    }
#pragma warning restore S2696 // Instance members should not write to "static" fields
}
