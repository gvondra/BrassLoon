using AutoMapper;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTaskTypeController : WorkTaskControllerBase
    {
        private readonly IWorkTaskTypeFactory _workTaskTypeFactory;
        private readonly IWorkTaskTypeSaver _workTaskTypeSaver;

        public WorkTaskTypeController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IWorkTaskTypeFactory workTaskTypeFactory,
            IWorkTaskTypeSaver workTaskTypeSaver) 
            :base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        { 
            _workTaskTypeFactory = workTaskTypeFactory;
            _workTaskTypeSaver = workTaskTypeSaver;
        }

        [HttpGet("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<WorkTaskType>), 200)]
        public async Task<IActionResult> GetAll([FromRoute] Guid? domainId)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        (await _workTaskTypeFactory.GetByDomainId(settings, domainId.Value))
                        .Select<IWorkTaskType, WorkTaskType>(t => mapper.Map<WorkTaskType>(t))
                        );
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
