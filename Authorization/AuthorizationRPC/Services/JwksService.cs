using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrassLoon.Authorization.Framework;
using BrassLoon.Interface.Authorization.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Protos = BrassLoon.Interface.Authorization.Protos;

namespace AuthorizationRPC.Services
{
    public class JwksService : Protos.JwksService.JwksServiceBase
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly ILogger<JwksService> _logger;
        private readonly ISigningKeyFactory _signingKeyFactory;

        public JwksService(SettingsFactory settingsFactory, ILogger<JwksService> logger, ISigningKeyFactory signingKeyFactory)
        {
            _settingsFactory = settingsFactory;
            _logger = logger;
            _signingKeyFactory = signingKeyFactory;
        }

        public override async Task<GetJwksResponse> Get(GetByDomainRequest request, ServerCallContext context)
        {
            try
            {
                Guid domainId;
                if (string.IsNullOrEmpty(request?.DomainId) || !Guid.TryParse(request.DomainId, out domainId))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing or invalid domain id \"{request?.DomainId}\"");
                CoreSettings settings = _settingsFactory.CreateCore();
                IEnumerable<ISigningKey> signingKeys = await _signingKeyFactory.GetByDomainId(settings, domainId);
                var jsonWebKeySet = new { Keys = new List<object>() };
                foreach (ISigningKey signingKey in signingKeys.Where(sk => sk.IsActive))
                {
                    RsaSecurityKey rsaSecurityKey = await signingKey.GetKey(settings, false);
                    JsonWebKey jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaSecurityKey);
                    jsonWebKey.KeyId = signingKey.SigningKeyId.ToString("N");
                    jsonWebKey.Alg = "RS512";
                    jsonWebKey.Use = "sig";
                    jsonWebKeySet.Keys.Add(jsonWebKey);
                }
                return new GetJwksResponse
                {
                    Token = JsonConvert.SerializeObject(jsonWebKeySet, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
                };
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
    }
}
