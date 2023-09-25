using BrassLoon.Authorization.Framework;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Authorization.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Protos = BrassLoon.Interface.Authorization.Protos;

namespace AuthorizationRPC.Services
{
    public class SigningKeyService : Protos.SigningKeyService.SigningKeyServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly SettingsFactory _settingsFactory;
        private readonly ILogger<SigningKeyService> _logger;
        private readonly ISigningKeyFactory _signingKeyFactory;
        private readonly ISigningKeySaver _signingKeySaver;

        public SigningKeyService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            SettingsFactory settingsFactory,
            ILogger<SigningKeyService> logger,
            ISigningKeyFactory signingKeyFactory,
            ISigningKeySaver signingKeySaver)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _settingsFactory = settingsFactory;
            _logger = logger;
            _signingKeyFactory = signingKeyFactory;
            _signingKeySaver = signingKeySaver;
        }

        public override async Task GetByDomain(GetByDomainRequest request, IServerStreamWriter<SigningKey> responseStream, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IEnumerable<ISigningKey> innerSigningKeys = await _signingKeyFactory.GetByDomainId(settings, domainId);
                foreach (ISigningKey innerSigningKey in innerSigningKeys)
                {
                    await responseStream.WriteAsync(Map(innerSigningKey));
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

        public override async Task<SigningKey> Create(SigningKey request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                ISigningKey innerSigningKey = _signingKeyFactory.Create(domainId);
                Map(request, innerSigningKey);
                await _signingKeySaver.Create(settings, innerSigningKey);
                return Map(innerSigningKey);
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

        public override async Task<SigningKey> Update(SigningKey request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request?.SigningKeyId) || !Guid.TryParse(request.SigningKeyId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid signing key id \"{request?.SigningKeyId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                ISigningKey innerSigningKey = await _signingKeyFactory.Get(settings, domainId, id);
                if (innerSigningKey == null)
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Not Found"), "Signing Key not found");
                Map(request, innerSigningKey);
                await _signingKeySaver.Update(settings, innerSigningKey);
                return Map(innerSigningKey);
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

        private static void Map(SigningKey signingKey, ISigningKey innerSigningKey)
            => innerSigningKey.IsActive = signingKey.IsActive ?? true;

        private static SigningKey Map(ISigningKey innerSigningKey)
        {
            return new SigningKey
            {
                CreateTimestamp = Timestamp.FromDateTime(innerSigningKey.CreateTimestamp),
                DomainId = innerSigningKey.DomainId.ToString("D"),
                IsActive = innerSigningKey.IsActive,
                SigningKeyId = innerSigningKey.SigningKeyId.ToString("D"),
                UpdateTimestamp = Timestamp.FromDateTime(innerSigningKey.UpdateTimestamp)
            };
        }
    }
}
