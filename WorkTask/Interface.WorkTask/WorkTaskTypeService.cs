using BrassLoon.Interface.WorkTask.Models;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkTaskTypeService : IWorkTaskTypeService
    {
        private static AsyncPolicy _getCache = CreateCachePolicy();
        private static AsyncPolicy _getByCodeCache = CreateCachePolicy();
        private static AsyncPolicy _getByWorkGroupIdCache = CreateCachePolicy();

        public async Task<WorkTaskType> Create(ISettings settings, WorkTaskType workTaskType)
        {
            if (workTaskType == null)
                throw new ArgumentNullException(nameof(workTaskType));
            if (!workTaskType.DomainId.HasValue || workTaskType.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskType.DomainId)} is null");
            Protos.WorkTaskType request = workTaskType.ToProto();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskTypeService.WorkTaskTypeServiceClient service = new Protos.WorkTaskTypeService.WorkTaskTypeServiceClient(channel);
                Protos.WorkTaskType response = await service.CreateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                ResetAllCaches();
                return WorkTaskType.Create(response);
            }
        }

        public Task<WorkTaskType> Get(ISettings settings, Guid domainId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            return _getCache.ExecuteAsync(
                (context) => GetUncached(settings, domainId, id),
                new Context($"{domainId:N}|{id:N}"));
        }

        private async Task<WorkTaskType> GetUncached(ISettings settings, Guid domainId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            Protos.GetWorkTaskTypeRequest request = new Protos.GetWorkTaskTypeRequest
            {
                DomainId = domainId.ToString("D"),
                WorkTaskTypeId = id.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskTypeService.WorkTaskTypeServiceClient service = new Protos.WorkTaskTypeService.WorkTaskTypeServiceClient(channel);
                Protos.WorkTaskType response = await service.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return WorkTaskType.Create(response);
            }
        }

        public async Task<List<WorkTaskType>> GetAll(ISettings settings, Guid domainId)
        {
            List<WorkTaskType> result = new List<WorkTaskType>();
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            Protos.GetByDomainRequest request = new Protos.GetByDomainRequest { DomainId = domainId.ToString("D") };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskTypeService.WorkTaskTypeServiceClient service = new Protos.WorkTaskTypeService.WorkTaskTypeServiceClient(channel);
                AsyncServerStreamingCall<Protos.WorkTaskType> streamingCall = service.GetAll(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await streamingCall.ResponseStream.MoveNext())
                {
                    result.Add(WorkTaskType.Create(streamingCall.ResponseStream.Current));
                }
            }
            return result;
        }

        public Task<WorkTaskType> GetByCode(ISettings settings, Guid domainId, string code)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));
            return _getByCodeCache.ExecuteAsync(
                (context) => GetByCodeUncached(settings, domainId, code),
                new Context($"{domainId:N}|{code}"));
        }

        private async Task<WorkTaskType> GetByCodeUncached(ISettings settings, Guid domainId, string code)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));
            Protos.GetWorkTaskTypeByCodeRequest request = new Protos.GetWorkTaskTypeByCodeRequest
            {
                DomainId = domainId.ToString("D"),
                Code = code
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskTypeService.WorkTaskTypeServiceClient service = new Protos.WorkTaskTypeService.WorkTaskTypeServiceClient(channel);
                Protos.WorkTaskType response = await service.GetByCodeAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return WorkTaskType.Create(response);
            }
        }

        public Task<List<WorkTaskType>> GetByWorkGroupId(ISettings settings, Guid domainId, Guid workGroupId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workGroupId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workGroupId));
            return _getByWorkGroupIdCache.ExecuteAsync(
                (context) => GetByWorkGroupIdUncached(settings, domainId, workGroupId),
                new Context($"{domainId:N}|{workGroupId:N}"));
        }

        private async Task<List<WorkTaskType>> GetByWorkGroupIdUncached(ISettings settings, Guid domainId, Guid workGroupId)
        {
            List<WorkTaskType> result = new List<WorkTaskType>();
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workGroupId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workGroupId));
            Protos.GetWorkTaskTypeByWorkGroupIdRequest request = new Protos.GetWorkTaskTypeByWorkGroupIdRequest
            {
                DomainId = domainId.ToString("D"),
                WorkGroupId = workGroupId.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskTypeService.WorkTaskTypeServiceClient service = new Protos.WorkTaskTypeService.WorkTaskTypeServiceClient(channel);
                AsyncServerStreamingCall<Protos.WorkTaskType> streamingCall = service.GetByWorkGroupId(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await streamingCall.ResponseStream.MoveNext())
                {
                    result.Add(WorkTaskType.Create(streamingCall.ResponseStream.Current));
                }
            }
            return result;
        }

        public async Task<WorkTaskType> Update(ISettings settings, WorkTaskType workTaskType)
        {
            if (workTaskType == null)
                throw new ArgumentNullException(nameof(workTaskType));
            if (!workTaskType.DomainId.HasValue || workTaskType.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskType.DomainId)} is null");
            if (!workTaskType.WorkTaskTypeId.HasValue || workTaskType.WorkTaskTypeId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskType.WorkTaskTypeId)} is null");
            Protos.WorkTaskType request = workTaskType.ToProto();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskTypeService.WorkTaskTypeServiceClient service = new Protos.WorkTaskTypeService.WorkTaskTypeServiceClient(channel);
                Protos.WorkTaskType response = await service.UpdateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                ResetAllCaches();
                return WorkTaskType.Create(response);
            }
        }

        private static AsyncPolicy CreateCachePolicy() => Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromMinutes(5)));

        private static void ResetAllCaches()
        {
            _getCache = CreateCachePolicy();
            _getByCodeCache = CreateCachePolicy();
            _getByWorkGroupIdCache = CreateCachePolicy();
        }
    }
}
