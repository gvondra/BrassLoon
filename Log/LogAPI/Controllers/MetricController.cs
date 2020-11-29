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
    public class MetricController : ControllerBase
    {
        [HttpPost()]
        public IActionResult Create([FromBody] LogModels.Metric metric)
        {
            IActionResult result = null;
            if (!metric.DomainId.HasValue || metric.DomainId.Value.Equals(Guid.Empty))
                result = BadRequest("Missing domain guid value");
            return result;
        }
    }
}
