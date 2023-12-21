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
    public class WorkTaskStatusService : IWorkTaskStatusService
    {
        private static AsyncPolicy _getCache = CreateCachePolicy();
        private static AsyncPolicy _getAllCache = CreateCachePolicy();

        public async Task<WorkTaskStatus> Create(ISettings settings, WorkTaskStatus workTaskStatus)
        {
            if (workTaskStatus == null)
                throw new ArgumentNullException(nameof(workTaskStatus));
            if (!workTaskStatus.DomainId.HasValue || workTaskStatus.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskStatus.DomainId)} is null");
            if (!workTaskStatus.WorkTaskTypeId.HasValue || workTaskStatus.WorkTaskTypeId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskStatus.WorkTaskTypeId)} is null");
            Protos.WorkTaskStatus request = workTaskStatus.ToProto();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskStatusService.WorkTaskStatusServiceClient service = new Protos.WorkTaskStatusService.WorkTaskStatusServiceClient(channel);
                Protos.WorkTaskStatus response = await service.CreateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                ResetAllCaches();
                return WorkTaskStatus.Create(response);
            }
        }

        public async Task Delete(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskTypeId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskTypeId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            Protos.DateWorkTaskStatusRequest request = new Protos.DateWorkTaskStatusRequest
            {
                DomainId = domainId.ToString("D"),
                WorkTaskStatusId = id.ToString("D"),
                WorkTaskTypeId = workTaskTypeId.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskStatusService.WorkTaskStatusServiceClient service = new Protos.WorkTaskStatusService.WorkTaskStatusServiceClient(channel);
                _ = await service.DeleteAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                ResetAllCaches();
            }
        }

        public Task<WorkTaskStatus> Get(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskTypeId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskTypeId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            return _getCache.ExecuteAsync(
                (context) => GetUncached(settings, domainId, workTaskTypeId, id),
                new Context($"{domainId:N}|{workTaskTypeId:N}{id:N}"));
        }

        private static async Task<WorkTaskStatus> GetUncached(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskTypeId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskTypeId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            Protos.GetWorkTaskStatusRequest request = new Protos.GetWorkTaskStatusRequest
            {
                DomainId = domainId.ToString("D"),
                WorkTaskStatusId = id.ToString("D"),
                WorkTaskTypeId = workTaskTypeId.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskStatusService.WorkTaskStatusServiceClient service = new Protos.WorkTaskStatusService.WorkTaskStatusServiceClient(channel);
                Protos.WorkTaskStatus workTaskStatus = await service.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return workTaskStatus != null ? WorkTaskStatus.Create(workTaskStatus) : null;
            }
        }

        public Task<List<WorkTaskStatus>> GetAll(ISettings settings, Guid domainId, Guid workTaskTypeId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskTypeId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskTypeId));
            return _getAllCache.ExecuteAsync(
                (context) => GetAllUncached(settings, domainId, workTaskTypeId),
                new Context($"{domainId:N}|{workTaskTypeId:N}"));
        }

        private static async Task<List<WorkTaskStatus>> GetAllUncached(ISettings settings, Guid domainId, Guid workTaskTypeId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskTypeId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskTypeId));
            Protos.GetAllWorkTaskStatusRequest request = new Protos.GetAllWorkTaskStatusRequest
            {
                DomainId = domainId.ToString("D"),
                WorkTaskTypeId = workTaskTypeId.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskStatusService.WorkTaskStatusServiceClient service = new Protos.WorkTaskStatusService.WorkTaskStatusServiceClient(channel);
                AsyncServerStreamingCall<Protos.WorkTaskStatus> streamingCall = service.GetAll(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                List<WorkTaskStatus> result = new List<WorkTaskStatus>();
                while (await streamingCall.ResponseStream.MoveNext())
                {
                    result.Add(
                        WorkTaskStatus.Create(streamingCall.ResponseStream.Current));
                }
                return result;
            }
        }

        public async Task<WorkTaskStatus> Update(ISettings settings, WorkTaskStatus workTaskStatus)
        {
            if (workTaskStatus == null)
                throw new ArgumentNullException(nameof(workTaskStatus));
            if (!workTaskStatus.DomainId.HasValue || workTaskStatus.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskStatus.DomainId)} is null");
            if (!workTaskStatus.WorkTaskTypeId.HasValue || workTaskStatus.WorkTaskTypeId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskStatus.WorkTaskTypeId)} is null");
            if (!workTaskStatus.WorkTaskStatusId.HasValue || workTaskStatus.WorkTaskStatusId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTaskStatus.WorkTaskStatusId)} is null");
            Protos.WorkTaskStatus request = workTaskStatus.ToProto();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskStatusService.WorkTaskStatusServiceClient service = new Protos.WorkTaskStatusService.WorkTaskStatusServiceClient(channel);
                Protos.WorkTaskStatus response = await service.UpdateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                ResetAllCaches();
                return WorkTaskStatus.Create(response);
            }
        }

        private static AsyncCachePolicy CreateCachePolicy() => Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromMinutes(5)));

        private static void ResetAllCaches()
        {
            _getCache = CreateCachePolicy();
            _getAllCache = CreateCachePolicy();
        }
    }
}
