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
        public async Task<IActionResult> GetAll([FromRoute] Guid? domainId, [FromQuery] string code)
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
                    IWorkTaskType innerWorkTaskType;
                    IMapper mapper = CreateMapper();
                    if (!string.IsNullOrEmpty(code))
                    {
                        innerWorkTaskType = await _workTaskTypeFactory.GetByDomainIdCode(settings, domainId.Value, code);
                        if (innerWorkTaskType == null)
                            result = Ok(null);
                        else
                            result = Ok(
                                mapper.Map<WorkTaskType>(innerWorkTaskType) 
                                );

                    }
                    else
                    {
                        result = Ok(
                            (await _workTaskTypeFactory.GetByDomainId(settings, domainId.Value))
                            .Select<IWorkTaskType, WorkTaskType>(t => mapper.Map<WorkTaskType>(t))
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

        [HttpGet("{domainId}/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<WorkTaskType>), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IWorkTaskType innerWorkTaskType = await _workTaskTypeFactory.Get(settings, id.Value);
                    if (innerWorkTaskType == null)
                        result = NotFound();
                    if (result == null)
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            mapper.Map<WorkTaskType>(innerWorkTaskType)
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

        [HttpGet("/api/WorkGroup/{domainId}/{workGroupId}/WorkTaskType")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<WorkTaskType>), 200)]
        public async Task<IActionResult> GetByWorkGroupId([FromRoute] Guid? domainId, [FromRoute] Guid? workGroupId)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!workGroupId.HasValue || workGroupId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing work group id parameter value");
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        (await _workTaskTypeFactory.GetByWorkGroupId(settings, workGroupId.Value))
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

        [NonAction]
        private IActionResult ValidateRequest(WorkTaskType workTaskType)
        {
            IActionResult result = null;
            if (result == null && workTaskType == null)
                result = BadRequest("Missing work task type body");
            if (result == null && string.IsNullOrEmpty(workTaskType?.Title))
                result = BadRequest("Missing work task type title value");
            return result;
        }

        [HttpPost("{domainId}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTaskType), 200)]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, [FromBody] WorkTaskType workTaskType)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                    result = ValidateRequest(workTaskType);
                if (result == null && string.IsNullOrEmpty(workTaskType.Code))
                    result = BadRequest("Missing work task type code value");
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IMapper mapper = CreateMapper();
                    IWorkTaskType innerWorkTaskType = _workTaskTypeFactory.Create(domainId.Value, workTaskType.Code);
                    mapper.Map(workTaskType, innerWorkTaskType);
                    await _workTaskTypeSaver.Create(settings, innerWorkTaskType);
                    result = Ok(
                        mapper.Map<WorkTaskType>(innerWorkTaskType)
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

        [HttpPut("{domainId}/{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTaskType), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? id, [FromBody] WorkTaskType workTaskType)
        {
            IActionResult result = null;
            try
            {
                if (result == null && (!id.HasValue || id.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing id parameter value");
                if (result == null && (!domainId.HasValue || domainId.Value.Equals(Guid.Empty)))
                    result = BadRequest("Missing domain id parameter value");
                if (result == null && !(await VerifyDomainAccount(domainId.Value)))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                if (result == null)
                    result = ValidateRequest(workTaskType);
                if (result == null)
                {
                    CoreSettings settings = CreateCoreSettings();
                    IWorkTaskType innerWorkTaskType = await _workTaskTypeFactory.Get(settings, id.Value);
                    if (innerWorkTaskType == null)
                        result = NotFound();
                    if (result == null)
                    {
                        IMapper mapper = CreateMapper();
                        mapper.Map(workTaskType, innerWorkTaskType);
                        await _workTaskTypeSaver.Update(settings, innerWorkTaskType);
                        result = Ok(
                            mapper.Map<WorkTaskType>(innerWorkTaskType)
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
