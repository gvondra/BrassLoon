using BrassLoon.Address.Framework;
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
    public class EmailAddressService : Protos.EmailAddressService.EmailAddressServiceBase
    {
        private readonly IDomainAcountAccessVerifier _domainAcountAccessVerifier;
        private readonly IMetaDataProcessor _metaDataProcessor;
        private readonly IMetricLogger _metricLogger;
        private readonly SettingsFactory _settingsFactory;
        private readonly IOptions<Settings> _settings;
        private readonly ILogger<EmailAddressService> _logger;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly IEmailAddressSaver _emailAddressSaver;

        public EmailAddressService(
            IDomainAcountAccessVerifier domainAcountAccessVerifier,
            IMetaDataProcessor metaDataProcessor,
            IMetricLogger metricLogger,
            SettingsFactory settingsFactory,
            IOptions<Settings> settings,
            ILogger<EmailAddressService> logger,
            IEmailAddressFactory emailAddressFactory,
            IEmailAddressSaver emailAddressSaver)
        {
            _domainAcountAccessVerifier = domainAcountAccessVerifier;
            _metaDataProcessor = metaDataProcessor;
            _metricLogger = metricLogger;
            _settingsFactory = settingsFactory;
            _settings = settings;
            _logger = logger;
            _emailAddressFactory = emailAddressFactory;
            _emailAddressSaver = emailAddressSaver;
        }

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<EmailAddress> Get(GetEmailAddressRequest request, ServerCallContext context)
        {
            DateTime start = DateTime.UtcNow;
            string status = string.Empty;
            try
            {
                Guid domainId;
                Guid id;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (string.IsNullOrEmpty(request.EmailAddressId) || !Guid.TryParse(request.EmailAddressId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid user id \"{request.EmailAddressId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IEmailAddress innerEmailAddress = await _emailAddressFactory.Get(settings, domainId, id);
                EmailAddress emailAddress = null;
                if (innerEmailAddress != null)
                    emailAddress = Map(innerEmailAddress);
                return emailAddress;
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
                    _metricLogger.ApiMethodStats(_logger, _settings.Value.LoggingDomainId.Value, "GetEmailAddress", status, start: start);
            }
        }

        [Authorize(Constants.POLICY_BL_AUTH)]
        public override async Task<EmailAddress> Save(EmailAddress request, ServerCallContext context)
        {
            DateTime start = DateTime.UtcNow;
            string status = string.Empty;
            try
            {
                Guid domainId;
                Guid id = Guid.Empty;
                bool newAddress = string.IsNullOrEmpty(request?.EmailAddressId);
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                if (!newAddress && !Guid.TryParse(request.EmailAddressId, out id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Invalid email address id \"{request.EmailAddressId}\"");
                string accessToken = _metaDataProcessor.GetBearerAuthorizationToken(context.RequestHeaders);
                if (!await _domainAcountAccessVerifier.HasAccess(
                    _settingsFactory.CreateAccount(accessToken),
                    domainId,
                    accessToken))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized"));
                }
                CoreSettings settings = _settingsFactory.CreateCore();
                IEmailAddress innerEmailAddress = newAddress ? _emailAddressFactory.Create(domainId) : await _emailAddressFactory.Get(settings, domainId, id);
                if (innerEmailAddress != null)
                {
                    Map(request, innerEmailAddress);
                    return Map(await _emailAddressSaver.Save(settings, innerEmailAddress));
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Email Address Not Found"));
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
                    _metricLogger.ApiMethodStats(_logger, _settings.Value.LoggingDomainId.Value, "SaveEmailAddress", status, start: start);
            }
        }

        private static EmailAddress Map(IEmailAddress innerEmailAddress)
        {
            return new EmailAddress
            {
                EmailAddressId = innerEmailAddress.EmailAddressId.ToString("D"),
                Address = innerEmailAddress.Address ?? string.Empty,
                CreateTimestamp = Timestamp.FromDateTime(innerEmailAddress.CreateTimestamp),
                DomainId = innerEmailAddress.DomainId.ToString("D")
            };
        }

        private static void Map(EmailAddress emailAddress, IEmailAddress innerEmailAddress)
            => innerEmailAddress.Address = emailAddress.Address ?? string.Empty;
    }
}
