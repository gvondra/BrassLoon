using BrassLoon.Interface.Log;
using BrassLoon.JwtUtility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace AccountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwksController : AccountControllerBase
    {
        private readonly ILogger<JwksController> _logger;

        public JwksController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<JwksController> logger)
            : base(settings, settingsFactory, exceptionService)
        {
            _logger = logger;
        }

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 90)]
        public IActionResult Get()
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }            
        }
    }
}
