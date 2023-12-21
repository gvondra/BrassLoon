using BrassLoon.Interface.Address.Models;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Address
{
    public class EmailAddressService : IEmailAddressService
    {
        public async Task<EmailAddress> Get(ISettings settings, Guid domainId, Guid id)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            Protos.GetEmailAddressRequest request = new Protos.GetEmailAddressRequest
            {
                DomainId = domainId.ToString("D"),
                EmailAddressId = id.ToString("D")
            };
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.EmailAddressService.EmailAddressServiceClient service = new Protos.EmailAddressService.EmailAddressServiceClient(channel);
                Protos.EmailAddress response = await service.GetAsync(request, await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return EmailAddress.Create(response);
            }
        }

        public async Task<EmailAddress> Save(ISettings settings, EmailAddress emailAddress)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (emailAddress == null || (emailAddress.DomainId ?? Guid.Empty) == Guid.Empty)
                throw new ArgumentException("Address missing domain id value");
            using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
            {
                Protos.EmailAddressService.EmailAddressServiceClient service = new Protos.EmailAddressService.EmailAddressServiceClient(channel);
                Protos.EmailAddress response = await service.SaveAsync(emailAddress.ToProto(), await RpcUtil.CreateMetaDataWithAuthHeader(settings));
                return EmailAddress.Create(response);
            }
        }
    }
}
