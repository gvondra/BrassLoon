﻿using LogModels = BrassLoon.Interface.Log.Models;
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
    public class ExceptionController : ControllerBase
    {
        [HttpPost()]
        public IActionResult Create([FromBody] LogModels.Exception exception)
        {
            IActionResult result = null;
            if (!exception.DomainId.HasValue || exception.DomainId.Value.Equals(Guid.Empty))
                result = BadRequest("Missing domain guid value");
            return result;
        }
    }
}
