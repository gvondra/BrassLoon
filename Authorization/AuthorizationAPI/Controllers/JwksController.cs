using BrassLoon.Authorization.Framework;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwksController : AuthorizationContollerBase
    {
        private readonly ISigningKeyFactory _signingKeyFactory;

        public JwksController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            ISigningKeyFactory signingKeyFactory)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        { 
            _signingKeyFactory = signingKeyFactory;
        }

        [HttpGet("{domainId}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 150)]
        public async Task<IActionResult> GetJwks([FromRoute] Guid? domainId)
        {
            IActionResult result = null;
            try
            {
                CoreSettings coreSettings = CreateCoreSettings();
                IEnumerable<ISigningKey> signingKeys = null;
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null)
                {
                    signingKeys = await _signingKeyFactory.GetByDomainId(coreSettings, domainId.Value);
                }                
                if (result == null && signingKeys != null)
                {
                    var jsonWebKeySet = new { Keys = new List<object>() };
                    foreach (ISigningKey signingKey in signingKeys)
                    {
                        RSAParameters rsaParameters = (await signingKey.GetKey(coreSettings)).ToRSA(false).ExportParameters(false);
                        RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(rsaParameters);
                        JsonWebKey jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaSecurityKey);
                        jsonWebKey.Alg = "RS512";
                        jsonWebKey.Use = "sig";
                        jsonWebKeySet.Keys.Add(jsonWebKey);
                    }
                    if (jsonWebKeySet.Keys.Count > 0) 
                        result = Content(
                            JsonConvert.SerializeObject(jsonWebKeySet, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), 
                            "appliation/json"
                            );
                }
                if (result == null)
                {
                    result = NoContent();
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            return result;
        }
    }
}
