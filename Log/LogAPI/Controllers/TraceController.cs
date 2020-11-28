using LogModels = BrassLoon.Interface.Log.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraceController : ControllerBase
    {
        [HttpPost("{domain}")]
        public IActionResult Create([FromRoute] Guid? domain, [FromBody] LogModels.Trace trace)
        {
            IActionResult result = null;
            if (!domain.HasValue || domain.Value.Equals(Guid.Empty))
                result = BadRequest("Missing domain guid value");
            return result;
        }
    }
}
