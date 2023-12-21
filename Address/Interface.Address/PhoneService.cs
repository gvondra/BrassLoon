using BrassLoon.Interface.Address.Models;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Address
{
    public class PhoneService : IPhoneService
    {
        public async Task<Phone> Get(ISettings settings, Guid domainId, Guid id)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            Protos.GetPhoneRequest request = new Protos.GetPhoneRequest
            {
                DomainId = domainId.ToString("D"),
                PhoneId = id.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.PhoneService.PhoneServiceClient service = new Protos.PhoneService.PhoneServiceClient(channel);
                Protos.Phone response = await service.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Phone.Create(response);
            }
        }

        public async Task<Phone> Save(ISettings settings, Phone phone)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (phone == null || (phone.DomainId ?? Guid.Empty) == Guid.Empty)
                throw new ArgumentException("Address missing domain id value");
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.PhoneService.PhoneServiceClient service = new Protos.PhoneService.PhoneServiceClient(channel);
                Protos.Phone response = await service.SaveAsync(phone.ToProto(), await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return Phone.Create(response);
            }
        }
    }
}
