﻿using BrassLoon.Address.Framework;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Address.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.Address.Protos;

namespace AddressRPC.Services
{
    public class AddressService : Protos.AddressService.AddressServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly IMetricLogger _metricLogger;
        private readonly SettingsFactory _settingsFactory;
        private readonly IOptions<Settings> _settings;
        private readonly ILogger<AddressService> _logger;
        private readonly IAddressFactory _addressFactory;
        private readonly IAddressSaver _addressSaver;

        public AddressService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            IMetricLogger metricLogger,
            SettingsFactory settingsFactory,
            IOptions<Settings> settings,
            ILogger<AddressService> logger,
            IAddressFactory addressFactory,
            IAddressSaver addressSaver)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _metricLogger = metricLogger;
            _settingsFactory = settingsFactory;
            _settings = settings;
            _logger = logger;
            _addressFactory = addressFactory;
            _addressSaver = addressSaver;
        }

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<Address> Get(GetAddressRequest request, ServerCallContext context)
        {
            DateTime start = DateTime.UtcNow;
            string status = string.Empty;
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.AddressId) || !Guid.TryParse(request.AddressId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid user id \"{request.AddressId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IAddress innerAddress = await _addressFactory.Get(settings, domainId, id);
                Address address = null;
                if (innerAddress != null)
                    address = Map(innerAddress);
                return address;
            }
            catch (RpcException ex)
            {
                status = ex.StatusCode.ToString();
                throw;
            }
            catch (Exception ex)
            {
                status = StatusCode.Internal.ToString();
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Internal System Error"), ex.Message);
            }
            finally
            {
                if (_settings.Value.LoggingDomainId.HasValue)
                    _metricLogger.ApiMethodStats(_logger, _settings.Value.LoggingDomainId.Value, "GetAddress", status, start: start);
            }
        }

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<Address> Save(Address request, ServerCallContext context)
        {
            DateTime start = DateTime.UtcNow;
            string status = string.Empty;
            try
            {
                Guid domainId;
                Guid id = Guid.Empty;
                bool newAddress = string.IsNullOrEmpty(request?.AddressId);
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (!newAddress && !Guid.TryParse(request.AddressId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Invalid address id \"{request.AddressId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                _logger.LogTrace($"Saving address {request.Addressee}, {request.Delivery}, {request.City} {request.Territory} {request.PostalCode}");
                IAddress innerAddress = newAddress ? _addressFactory.Create(domainId) : await _addressFactory.Get(settings, domainId, id);
                if (innerAddress != null)
                {
                    Map(request, innerAddress);
                    _logger.LogTrace($"Mapped address (before save) {innerAddress.Addressee}, {innerAddress.Delivery}, {innerAddress.City} {innerAddress.Territory} {innerAddress.PostalCode}");
                    return Map(await _addressSaver.Save(settings, innerAddress));
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Address Not Found"));
                }
            }
            catch (RpcException ex)
            {
                status = ex.StatusCode.ToString();
                throw;
            }
            catch (Exception ex)
            {
                status = StatusCode.Internal.ToString();
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Internal System Error"), ex.Message);
            }
            finally
            {
                if (_settings.Value.LoggingDomainId.HasValue)
                    _metricLogger.ApiMethodStats(_logger, _settings.Value.LoggingDomainId.Value, "SaveAddress", status, start: start);
            }
        }

        private static Address Map(IAddress innerAddress)
        {
            return new Address
            {
                Addressee = innerAddress.Addressee ?? string.Empty,
                AddressId = innerAddress.AddressId.ToString("D"),
                Attention = innerAddress.Attention ?? string.Empty,
                City = innerAddress.City ?? string.Empty,
                Country = innerAddress.Country ?? string.Empty,
                County = innerAddress.County ?? string.Empty,
                CreateTimestamp = Timestamp.FromDateTime(innerAddress.CreateTimestamp),
                Delivery = innerAddress.Delivery ?? string.Empty,
                DomainId = innerAddress.DomainId.ToString("D"),
                PostalCode = innerAddress.PostalCode ?? string.Empty,
                Territory = innerAddress.Territory ?? string.Empty
            };
        }

        private static void Map(Address address, IAddress innerAddress)
        {
            innerAddress.Attention = address.Attention ?? string.Empty;
            innerAddress.Addressee = address.Addressee ?? string.Empty;
            innerAddress.City = address.City ?? string.Empty;
            innerAddress.Country = address.Country ?? string.Empty;
            innerAddress.County = address.County ?? string.Empty;
            innerAddress.Delivery = address.Delivery ?? string.Empty;
            innerAddress.PostalCode = address.PostalCode ?? string.Empty;
            innerAddress.Territory = address.Territory ?? string.Empty;
        }
    }
}
