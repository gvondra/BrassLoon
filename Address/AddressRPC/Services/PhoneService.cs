﻿using BrassLoon.Address.Framework;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Address.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.Address.Protos;

namespace AddressRPC.Services
{
    public class PhoneService : Protos.PhoneService.PhoneServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILogger<PhoneService> _logger;
        private readonly IPhoneFactory _phoneFactory;
        private readonly IPhoneSaver _phoneSaver;

        public PhoneService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            SettingsFactory settingsFactory,
            ILogger<PhoneService> logger,
            IPhoneFactory phoneFactory,
            IPhoneSaver phoneSaver)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _phoneFactory = phoneFactory;
            _phoneSaver = phoneSaver;
        }

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<Phone> Get(GetPhoneRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.PhoneId) || !Guid.TryParse(request.PhoneId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid user id \"{request.PhoneId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IPhone innerPhone = await _phoneFactory.Get(settings, domainId, id);
                Phone phone = null;
                if (innerPhone != null)
                    phone = Map(innerPhone);
                return phone;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Internal System Error"), ex.Message);
            }
        }

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<Phone> Save(Phone request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id = Guid.Empty;
                bool newAddress = string.IsNullOrEmpty(request?.PhoneId);
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (!newAddress && !Guid.TryParse(request.PhoneId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Invalid email address id \"{request.PhoneId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IPhone innerPhone = newAddress ? _phoneFactory.Create(domainId) : await _phoneFactory.Get(settings, domainId, id);
                if (innerPhone != null)
                {
                    Map(request, innerPhone);
                    return Map(await _phoneSaver.Save(settings, innerPhone));
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Email Address Not Found"));
                }
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Internal System Error"), ex.Message);
            }
        }

        private static Phone Map(IPhone innerPhone)
        {
            return new Phone
            {
                PhoneId = innerPhone.PhoneId.ToString("D"),
                Number = innerPhone.Number ?? string.Empty,
                CountryCode = innerPhone.CountryCode ?? string.Empty,
                CreateTimestamp = Timestamp.FromDateTime(innerPhone.CreateTimestamp),
                DomainId = innerPhone.DomainId.ToString("D")
            };
        }

        private static void Map(Phone phone, IPhone innerPhone)
        {
            innerPhone.Number = phone.Number ?? string.Empty;
            innerPhone.CountryCode = phone.CountryCode ?? string.Empty;
        }
    }
}
