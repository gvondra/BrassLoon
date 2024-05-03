using BrassLoon.Interface.WorkTask.Models;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkTaskService : IWorkTaskService
    {
        public async Task<ClaimWorkTaskResponse> Claim(ISettings settings, Guid domainId, Guid id, string assignToUserId, DateTime? assignedDate = null)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            Protos.ClaimWorkTaskRequest request = new Protos.ClaimWorkTaskRequest
            {
                AssignedDate = assignedDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty,
                AssignToUserId = assignToUserId ?? string.Empty,
                DomainId = domainId.ToString("D"),
                WorkTaskId = id.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskService.WorkTaskServiceClient service = new Protos.WorkTaskService.WorkTaskServiceClient(channel);
                Protos.ClaimWorkTaskResponse response = await service.ClaimAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return ClaimWorkTaskResponse.Create(response);
            }
        }

        public async Task<Models.WorkTask> Create(ISettings settings, Models.WorkTask workTask)
        {
            if (workTask == null)
                throw new ArgumentNullException(nameof(workTask));
            if (!workTask.DomainId.HasValue || workTask.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTask.DomainId)} is null");
            Protos.WorkTask request = workTask.ToProto();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskService.WorkTaskServiceClient service = new Protos.WorkTaskService.WorkTaskServiceClient(channel);
                Protos.WorkTask response = await service.CreateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Models.WorkTask.Create(response);
            }
        }

        public async Task<Models.WorkTask> Get(ISettings settings, Guid domainId, Guid id)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            Protos.GetWorkTaskRequest request = new Protos.GetWorkTaskRequest
            {
                DomainId = domainId.ToString("D"),
                WorkTaskId = id.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskService.WorkTaskServiceClient service = new Protos.WorkTaskService.WorkTaskServiceClient(channel);
                Protos.WorkTask response = await service.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Models.WorkTask.Create(response);
            }
        }

        public Task<IAsyncEnumerable<Models.WorkTask>> GetAll(ISettings settings, Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            Protos.GetByDomainRequest request = new Protos.GetByDomainRequest
            {
                DomainId = domainId.ToString("D")
            };
            return Task.FromResult<IAsyncEnumerable<Models.WorkTask>>(
                new StreamEnumerable<Protos.WorkTask, Models.WorkTask>(
                    settings,
                    async channel =>
                    {
                        Protos.WorkTaskService.WorkTaskServiceClient service = new Protos.WorkTaskService.WorkTaskServiceClient(channel);
                        return service.GetAll(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                    },
                    Models.WorkTask.Create));
        }

        public async Task<List<Models.WorkTask>> GetByContext(
            ISettings settings,
            Guid domainId,
            short referenceType,
            string referenceValue,
            bool? includeClosed = null)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (string.IsNullOrEmpty(referenceValue))
                throw new ArgumentNullException(nameof(referenceValue));
            List<Models.WorkTask> result = new List<Models.WorkTask>();
            Protos.GetWorkTaskByContextRequest request = new Protos.GetWorkTaskByContextRequest
            {
                DomainId = domainId.ToString("D"),
                ReferenceType = referenceType,
                ReferenceValue = referenceValue,
                IncludeClosed = includeClosed ?? false
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskService.WorkTaskServiceClient service = new Protos.WorkTaskService.WorkTaskServiceClient(channel);
                AsyncServerStreamingCall<Protos.WorkTask> streamingCall = service.GetByContext(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await streamingCall.ResponseStream.MoveNext())
                {
                    result.Add(
                        Models.WorkTask.Create(streamingCall.ResponseStream.Current));
                }
            }
            return result;
        }

        public async Task<List<Models.WorkTask>> GetByWorkGroupId(ISettings settings, Guid domainId, Guid workGroupId, bool? includeClosed = null)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workGroupId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workGroupId));
            List<Models.WorkTask> result = new List<Models.WorkTask>();
            Protos.GetWorkTaskByWorkGroupIdRequest request = new Protos.GetWorkTaskByWorkGroupIdRequest
            {
                DomainId = domainId.ToString("D"),
                WorkGroupId = workGroupId.ToString("D"),
                IncludeClosed = includeClosed ?? false
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskService.WorkTaskServiceClient service = new Protos.WorkTaskService.WorkTaskServiceClient(channel);
                AsyncServerStreamingCall<Protos.WorkTask> streamingCall = service.GetByWorkGroupId(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await streamingCall.ResponseStream.MoveNext())
                {
                    result.Add(
                        Models.WorkTask.Create(streamingCall.ResponseStream.Current));
                }
            }
            return result;
        }

        private static Protos.PatchWorkTaskRequest CreatePatchRequest(Guid domainId, Dictionary<string, object> patchItem)
        {
            Protos.PatchWorkTaskRequest request = new Protos.PatchWorkTaskRequest
            {
                DomainId = domainId.ToString("D")
            };
            foreach (KeyValuePair<string, object> kvp in patchItem)
            {
                request.Data.Add(kvp.Key, kvp.Value?.ToString() ?? default);
            }
            return request;
        }

        public async Task<List<Models.WorkTask>> Patch(ISettings settings, Guid domainId, IEnumerable<Dictionary<string, object>> patchData)
        {
            if (patchData == null)
                throw new ArgumentNullException(nameof(patchData));
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            List<Models.WorkTask> result = new List<Models.WorkTask>();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskService.WorkTaskServiceClient service = new Protos.WorkTaskService.WorkTaskServiceClient(channel);
                AsyncDuplexStreamingCall<Protos.PatchWorkTaskRequest, Protos.WorkTask> streamingCall = service.Patch(await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                foreach (Dictionary<string, object> patchItem in patchData)
                {
                    await streamingCall.RequestStream.WriteAsync(
                        CreatePatchRequest(domainId, patchItem));
                }
                await streamingCall.RequestStream.CompleteAsync();
                while (await streamingCall.ResponseStream.MoveNext())
                {
                    result.Add(
                        Models.WorkTask.Create(streamingCall.ResponseStream.Current));
                }
            }
            return result;
        }

        public async Task<Models.WorkTask> Update(ISettings settings, Models.WorkTask workTask)
        {
            if (workTask == null)
                throw new ArgumentNullException(nameof(workTask));
            if (!workTask.DomainId.HasValue || workTask.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTask.DomainId)} is null");
            if (!workTask.WorkTaskId.HasValue || workTask.WorkTaskId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workTask.WorkTaskId)} is null");
            Protos.WorkTask request = workTask.ToProto();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkTaskService.WorkTaskServiceClient service = new Protos.WorkTaskService.WorkTaskServiceClient(channel);
                Protos.WorkTask response = await service.UpdateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Models.WorkTask.Create(response);
            }
        }
    }
}
