using AutoMapper;
using BrassLoon.CommonAPI;
using BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkTaskAPI.Controllers
{
    [Route("api/WorkTaskType/{domainId}/{workTaskTypeId}/Status")]
    [ApiController]
    public class WorkTaskStatusController : WorkTaskControllerBase
    {
        private readonly ILogger<WorkTaskStatusController> _logger;
        private readonly IWorkTaskStatusFactory _workTaskStatusFactory;
        private readonly IWorkTaskTypeFactory _workTaskTypeFactory;
        private readonly IWorkTaskTypeSaver _workTaskTypeSaver;

        public WorkTaskStatusController(IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            IExceptionService exceptionService,
            ILogger<WorkTaskStatusController> logger,
            MapperFactory mapperFactory,
            IDomainService domainService,
            IWorkTaskStatusFactory workTaskStatusFactory,
            IWorkTaskTypeFactory workTaskTypeFactory,
            IWorkTaskTypeSaver workTaskTypeSaver)
            : base(settings, settingsFactory, exceptionService, mapperFactory, domainService)
        {
            _logger = logger;
            _workTaskStatusFactory = workTaskStatusFactory;
            _workTaskTypeFactory = workTaskTypeFactory;
            _workTaskTypeSaver = workTaskTypeSaver;
        }

        [HttpGet()]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(List<WorkTaskStatus>), 200)]
        public async Task<IActionResult> GetAll([FromRoute] Guid? domainId, [FromRoute] Guid? workTaskTypeId)
        {
            IActionResult result;
            try
            {
                if (!workTaskTypeId.HasValue || workTaskTypeId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing work task type id parameter value");
                }
                else if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = CreateCoreSettings();
                    IMapper mapper = CreateMapper();
                    result = Ok(
                        (await _workTaskStatusFactory.GetByWorkTaskTypeId(settings, workTaskTypeId.Value))
                        .Select<IWorkTaskStatus, WorkTaskStatus>(t => mapper.Map<WorkTaskStatus>(t))
                        );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpGet("{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTaskStatus), 200)]
        public async Task<IActionResult> Get([FromRoute] Guid? domainId, [FromRoute] Guid? workTaskTypeId, [FromRoute] Guid? id)
        {
            IActionResult result;
            try
            {
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else if (!workTaskTypeId.HasValue || workTaskTypeId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing work task type id parameter value");
                }
                else if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    CoreSettings settings = CreateCoreSettings();
                    IWorkTaskStatus innerWorkTaskStatus = await _workTaskStatusFactory.Get(settings, id.Value);
                    if (innerWorkTaskStatus == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        result = Ok(
                            mapper.Map<WorkTaskStatus>(innerWorkTaskStatus)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [NonAction]
        private IActionResult ValidateRequest(WorkTaskStatus workTaskStatus)
        {
            IActionResult result = null;
            if (result == null && workTaskStatus == null)
                result = BadRequest("Missing work task status body");
            if (result == null && string.IsNullOrEmpty(workTaskStatus?.Name))
                result = BadRequest("Missing work task status name value");
            if (result == null && !workTaskStatus.IsClosedStatus.HasValue)
                result = BadRequest("Missing work task status is closed value");
            return result;
        }

        [HttpPost()]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTaskStatus), 200)]
        public async Task<IActionResult> Create([FromRoute] Guid? domainId, [FromRoute] Guid? workTaskTypeId, [FromBody] WorkTaskStatus workTaskStatus)
        {
            IActionResult result;
            try
            {
                CoreSettings settings = CreateCoreSettings();
                IWorkTaskType innerWorkTaskType = null;
                if (!workTaskTypeId.HasValue || workTaskTypeId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing work task type id parameter value");
                else if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing domain id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                else if (string.IsNullOrEmpty(workTaskStatus?.Code))
                    result = BadRequest("Missing work task status code value");
                else
                    result = ValidateRequest(workTaskStatus);
                if (result == null)
                {
                    innerWorkTaskType = await _workTaskTypeFactory.Get(settings, workTaskTypeId.Value);
                    if (innerWorkTaskType == null)
                        result = NotFound();
                }
                if (result == null)
                {
                    IWorkTaskStatus innerWorkTaskStatus = innerWorkTaskType.CreateWorkTaskStatus(workTaskStatus.Code);
                    IMapper mapper = CreateMapper();
                    mapper.Map(workTaskStatus, innerWorkTaskStatus);
                    await _workTaskTypeSaver.Create(settings, innerWorkTaskStatus);
                    result = Ok(
                        mapper.Map<WorkTaskStatus>(innerWorkTaskStatus)
                        );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        [ProducesResponseType(typeof(WorkTaskStatus), 200)]
        public async Task<IActionResult> Update([FromRoute] Guid? domainId, [FromRoute] Guid? workTaskTypeId, [FromRoute] Guid? id, [FromBody] WorkTaskStatus workTaskStatus)
        {
            IActionResult result;
            try
            {
                CoreSettings settings = CreateCoreSettings();
                IWorkTaskType innerWorkTaskType = null;
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing id parameter value");
                else if (!workTaskTypeId.HasValue || workTaskTypeId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing work task type id parameter value");
                else if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                    result = BadRequest("Missing domain id parameter value");
                else if (!await VerifyDomainAccount(domainId.Value))
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                else
                    result = ValidateRequest(workTaskStatus);
                if (result == null)
                {
                    innerWorkTaskType = await _workTaskTypeFactory.Get(settings, workTaskTypeId.Value);
                    if (innerWorkTaskType == null)
                        result = NotFound();
                }
                if (result == null)
                {
                    IWorkTaskStatus innerWorkTaskStatus = await _workTaskStatusFactory.Get(settings, id.Value);
                    if (innerWorkTaskStatus == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        IMapper mapper = CreateMapper();
                        mapper.Map(workTaskStatus, innerWorkTaskStatus);
                        await _workTaskTypeSaver.Update(settings, innerWorkTaskStatus);
                        result = Ok(
                            mapper.Map<WorkTaskStatus>(innerWorkTaskStatus)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }

        [HttpDelete("{id}")]
        [Authorize(Constants.POLICY_BL_AUTH)]
        public async Task<IActionResult> Delete([FromRoute] Guid? domainId, [FromRoute] Guid? workTaskTypeId, [FromRoute] Guid? id)
        {
            IActionResult result = null;
            try
            {
                CoreSettings settings = CreateCoreSettings();
                IWorkTaskType innerWorkTaskType = null;
                if (!id.HasValue || id.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing id parameter value");
                }
                else if (!workTaskTypeId.HasValue || workTaskTypeId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing work task type id parameter value");
                }
                else if (!domainId.HasValue || domainId.Value.Equals(Guid.Empty))
                {
                    result = BadRequest("Missing domain id parameter value");
                }
                else if (!await VerifyDomainAccount(domainId.Value))
                {
                    result = StatusCode(StatusCodes.Status401Unauthorized);
                }
                else
                {
                    innerWorkTaskType = await _workTaskTypeFactory.Get(settings, workTaskTypeId.Value);
                    if (innerWorkTaskType == null)
                        result = NotFound();
                }
                if (result == null)
                {
                    IWorkTaskStatus innerWorkTaskStatus = await _workTaskStatusFactory.Get(settings, id.Value);
                    if (innerWorkTaskStatus == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        if (innerWorkTaskStatus.WorkTaskCount > 0)
                            result = BadRequest("Unable to delete status in use");
                    }
                }
                if (result == null)
                {
                    await _workTaskTypeSaver.DeleteStatus(settings, id.Value);
                    result = Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
