using AutoMapper;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using BrassLoon.WorkTask.Core;
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
    public class WorkGroupController : WorkTaskControllerBase
    {
        private readonly IWorkGroupFactory _workGroupFactory;
        private readonly IWorkGroupSaver _workGroupSaver;

        public WorkGroupController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IWorkGroupFactory workGroupFactory,
            IWorkGroupSaver workGroupSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        { 
            _workGroupFactory = workGroupFactory;
            _workGroupSaver = workGroupSaver;
        }

        [HttpGet("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<WorkGroup>), 200)]
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
                        (await _workGroupFactory.GetByDomainId(settings, domainId.Value))
                        .Select<IWorkGroup, WorkGroup>(t => mapper.Map<WorkGroup>(t))
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

        [HttpGet("{domainId}/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkGroup), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IWorkGroup innerWorkGroup = await _workGroupFactory.Get(settings, id.Value);
                    if (innerWorkGroup == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            mapper.Map<WorkGroup>(innerWorkGroup)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        IActionResult ValidateRequest(WorkGroup workGroup)
        {
            IActionResult result = null;
            if (result == null && workGroup == null)
                result = BadRequest("Missing work group body");
            if (result == null && string.IsNullOrEmpty(workGroup?.Title))
                result = BadRequest("Missing work group title value");
            return result;
        }

        [HttpPost("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkGroup), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromBody] WorkGroup workGroup)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                    result = ValidateRequest(workGroup);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IWorkGroup innerWorkGroup = _workGroupFactory.Create(domainId.Value);
                    IMapper mapper = CreateMapper();
                    mapper.Map(workGroup, innerWorkGroup);
                    await _workGroupSaver.Create(settings, innerWorkGroup);
                    result = Ok(mapper.Map<WorkGroup>(innerWorkGroup));
                }
            }
            catch (Exception ex)
            {
                await LogException(ex);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{domainId}/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkGroup), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromBody] WorkGroup workGroup)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                    result = ValidateRequest(workGroup);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IWorkGroup innerWorkGroup = await _workGroupFactory.Get(settings, id.Value);
                    if (innerWorkGroup == null)
                        result = NotFound();
                    else
                    {
                        IMapper mapper = CreateMapper();
                        mapper.Map(workGroup, innerWorkGroup);
                        await _workGroupSaver.Update(settings, innerWorkGroup);
                        result = Ok(
                            mapper.Map<WorkGroup>(innerWorkGroup)
                            );
                    }
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
