using BrassLoon.Interface.Log;
using BrassLoon.JwtUtility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwksController : AccountControllerBase
    {
        private readonly Lazy<IExceptionService> _exceptionService;

        public JwksController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            Lazy<IExceptionService> exceptionService)
            : base(settings, settingsFactory)
        {
            _exceptionService = exceptionService;
        }

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 90)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var jsonWebKeySet = new { Keys = new List<object>() };
                RsaSecurityKey securityKey = RsaSecurityKeySerializer.GetSecurityKey(_settings.Value.TknCsp);
                JsonWebKey jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(securityKey);
                jsonWebKeySet.Keys.Add(jsonWebKey);
                return Content(JsonConvert.SerializeObject(jsonWebKeySet, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), "appliation/json");
            }
            catch (Exception ex)
            {
                await LogException(ex, _exceptionService.Value, _settingsFactory, _settings.Value);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }            
        }
    }
}
