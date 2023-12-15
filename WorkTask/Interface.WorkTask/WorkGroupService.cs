using BrassLoon.Interface.WorkTask.Models;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkGroupService : IWorkGroupService
    {
        public async Task AddWorkTaskTypeLink(ISettings settings, Guid domainId, Guid workGroupId, Guid workTaskTypeId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workGroupId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workGroupId));
            if (workTaskTypeId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskTypeId));
            Protos.WorkGroupTaskTypeLinkRequest request = new Protos.WorkGroupTaskTypeLinkRequest
            {
                DomainId = domainId.ToString("D"),
                WorkGroupId = workGroupId.ToString("D"),
                WorkTaskTypeId = workTaskTypeId.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkGroupService.WorkGroupServiceClient service = new Protos.WorkGroupService.WorkGroupServiceClient(channel);
                _ = await service.AddWorkTaskTypeLinkAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
            }
        }

        public async Task<WorkGroup> Create(ISettings settings, WorkGroup workGroup)
        {
            if (workGroup == null)
                throw new ArgumentNullException(nameof(workGroup));
            if (!workGroup.DomainId.HasValue || workGroup.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workGroup.DomainId)} is null");
            Protos.WorkGroup request = workGroup.ToProto();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkGroupService.WorkGroupServiceClient service = new Protos.WorkGroupService.WorkGroupServiceClient(channel);
                Protos.WorkGroup response = await service.CreateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return WorkGroup.Create(response);
            }
        }

        public async Task DeleteWorkTaskTypeLink(ISettings settings, Guid domainId, Guid workGroupId, Guid workTaskTypeId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workGroupId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workGroupId));
            if (workTaskTypeId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskTypeId));
            Protos.WorkGroupTaskTypeLinkRequest request = new Protos.WorkGroupTaskTypeLinkRequest
            {
                DomainId = domainId.ToString("D"),
                WorkGroupId = workGroupId.ToString("D"),
                WorkTaskTypeId = workTaskTypeId.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkGroupService.WorkGroupServiceClient service = new Protos.WorkGroupService.WorkGroupServiceClient(channel);
                _ = await service.DeleteWorkTaskTypeLinkAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
            }
        }

        public async Task<WorkGroup> Get(ISettings settings, Guid domainId, Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            Protos.GetWorkGroupRequest request = new Protos.GetWorkGroupRequest
            {
                DomainId = domainId.ToString("D"),
                WorkGroupId = id.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkGroupService.WorkGroupServiceClient service = new Protos.WorkGroupService.WorkGroupServiceClient(channel);
                Protos.WorkGroup response = await service.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return WorkGroup.Create(response);
            }
        }

        public async Task<List<WorkGroup>> GetAll(ISettings settings, Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            Protos.GetByDomainRequest request = new Protos.GetByDomainRequest
            {
                DomainId = domainId.ToString("D")
            };
            List<WorkGroup> result = new List<WorkGroup>();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkGroupService.WorkGroupServiceClient service = new Protos.WorkGroupService.WorkGroupServiceClient(channel);
                AsyncServerStreamingCall<Protos.WorkGroup> streamingCall = service.GetAll(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await streamingCall.ResponseStream.MoveNext())
                {
                    result.Add(
                        WorkGroup.Create(streamingCall.ResponseStream.Current));
                }
            }
            return result;
        }

        public async Task<List<WorkGroup>> GetByMemberUserId(ISettings settings, Guid domainId, string userId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));
            Protos.GetWorkGroupByMeemberUserIdRequest request = new Protos.GetWorkGroupByMeemberUserIdRequest
            {
                DomainId = domainId.ToString("D"),
                UserId = userId
            };
            List<WorkGroup> result = new List<WorkGroup>();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkGroupService.WorkGroupServiceClient service = new Protos.WorkGroupService.WorkGroupServiceClient(channel);
                AsyncServerStreamingCall<Protos.WorkGroup> streamingCall = service.GetByMemberUserId(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                while (await streamingCall.ResponseStream.MoveNext())
                {
                    result.Add(
                        WorkGroup.Create(streamingCall.ResponseStream.Current));
                }
            }
            return result;
        }

        public async Task<WorkGroup> Update(ISettings settings, WorkGroup workGroup)
        {
            if (workGroup == null)
                throw new ArgumentNullException(nameof(workGroup));
            if (!workGroup.DomainId.HasValue || workGroup.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workGroup.DomainId)} is null");
            if (!workGroup.WorkGroupId.HasValue || workGroup.WorkGroupId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"{nameof(workGroup.WorkGroupId)} is null");
            Protos.WorkGroup request = workGroup.ToProto();
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.WorkGroupService.WorkGroupServiceClient service = new Protos.WorkGroupService.WorkGroupServiceClient(channel);
                Protos.WorkGroup response = await service.UpdateAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return WorkGroup.Create(response);
            }
        }
    }
}
