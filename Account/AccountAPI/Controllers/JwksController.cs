using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwksController : ControllerBase
    {
        private IOptions<Settings> _settings;

        public JwksController(IOptions<Settings> settings)
        {
            _settings = settings;
        }

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 90)]
        public IActionResult Get()
        {
            var jsonWebKeySet = new { Keys = new List<object>() };
            RsaSecurityKey securityKey = GetSecurityKey(_settings.Value.TknCsp);
            JsonWebKey jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(securityKey);
            jsonWebKeySet.Keys.Add(jsonWebKey);
            return Content(JsonConvert.SerializeObject(jsonWebKeySet, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), "appliation/json");
        }

        public static RsaSecurityKey GetSecurityKey(string tknCsp)
        {
            dynamic tknCspData;
            try
            {
                tknCspData = JsonConvert.DeserializeObject(tknCsp);
            }   
            catch (Exception ex)
            {
                throw new ApplicationException(tknCsp, ex);
            }
            RSAParameters rsaParameters = new RSAParameters
            {
                D = Base64UrlEncoder.DecodeBytes((string)tknCspData.d),
                DP = Base64UrlEncoder.DecodeBytes((string)tknCspData.dp),
                DQ = Base64UrlEncoder.DecodeBytes((string)tknCspData.dq),
                Exponent = Base64UrlEncoder.DecodeBytes((string)tknCspData.e),
                InverseQ = Base64UrlEncoder.DecodeBytes((string)tknCspData.qi),
                Modulus = Base64UrlEncoder.DecodeBytes((string)tknCspData.n),
                P = Base64UrlEncoder.DecodeBytes((string)tknCspData.p),
                Q = Base64UrlEncoder.DecodeBytes((string)tknCspData.q)
            };
            return new RsaSecurityKey(RSA.Create(rsaParameters).ExportParameters(false));
        }
    }
}
