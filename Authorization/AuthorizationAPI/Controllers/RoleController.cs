using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : AuthorizationContollerBase
    {
        public RoleController(IOptions<Settings> settings,
            SettingsFactory settingsFactory)
            : base(settings, settingsFactory) { }
    }
}
