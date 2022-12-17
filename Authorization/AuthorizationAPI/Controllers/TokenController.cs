using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : AuthorizationContollerBase
    {
        public TokenController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        { }
    }
}
