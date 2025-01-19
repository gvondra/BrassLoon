using BrassLoon.Interface.WorkTask.Models;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkTaskStatusService : IWorkTaskStatusService
    {
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
                return WorkTaskStatus.Create(response);
            }
        }
    }
}
